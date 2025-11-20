using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class ReportRepository : IReportRepository
    {
        private readonly AppDbContext _context;

        public ReportRepository(AppDbContext context)
        {
            _context = context;
        }

        // 1. Tổng doanh thu
        public async Task<decimal> GetTotalRevenueAsync(int storeId)
        {
            return await (from o in _context.Orders
                          join d in _context.Dealers on o.DealerId equals d.DealerId
                          where o.Status == OrderEnum.Completed && d.StoreId == storeId
                          select (decimal?)o.TotalPrice)
                          .SumAsync() ?? 0;
        }

        // 2. Tổng khách hàng
        public async Task<int> GetTotalCustomersAsync(int storeId)
        {
            return await _context.StoreCustomers
                .Where(sc => sc.StoreId == storeId)
                .Select(sc => sc.CustomerId)
                .Distinct()
                .CountAsync();
        }

        // 3. Tổng đơn hàng
        public async Task<int> GetTotalOrdersAsync(int storeId)
        {
            return await _context.Orders
                .Include(o => o.Dealer)
                .Where(o => o.Status == OrderEnum.Completed && o.Dealer.StoreId == storeId)
                .CountAsync();
        }

        // 4. Doanh thu theo tháng
        public async Task<List<(string Month, decimal Revenue)>> GetRevenueByMonthAsync(int storeId)
        {
            var data = await _context.Orders
                .Include(o => o.Dealer)
                .Where(o => o.Status == OrderEnum.Completed
                            && o.OrderDate.HasValue
                            && o.Dealer.StoreId == storeId)
                .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(x => (decimal?)x.TotalPrice) ?? 0
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            return data.Select(x => ($"{x.Month:D2}/{x.Year}", x.Revenue)).ToList();
        }

        // 5. Xe bán chạy nhất
        public async Task<List<(string ModelName, int Quantity)>> GetTopVehiclesAsync(int storeId, int top = 5)
        {
            var query = _context.Quotes
                .Include(q => q.Vehicle)
                .Include(q => q.Dealer)
                .Where(q => q.Status == QuoteEnum.Accepted && q.Dealer.StoreId == storeId);

            var data = await query
                .GroupBy(q => q.Vehicle.ModelName)
                .Select(g => new
                {
                    ModelName = g.Key,
                    Quantity = g.Count()
                })
                .OrderByDescending(x => x.Quantity)
                .Take(top)
                .ToListAsync();

            return data.Select(x => (x.ModelName, x.Quantity)).ToList();
        }

        // 6. Tồn kho
        public async Task<List<(string ModelName, int Stock)>> GetInventoryAsync(int storeId)
        {
            var data = await _context.Storages
                .Include(s => s.Vehicle)
                .Where(s => s.StoreId == storeId)
                .GroupBy(s => s.Vehicle.ModelName)
                .Select(g => new
                {
                    ModelName = g.Key,
                    Stock = g.Sum(s => s.QuantityAvailable ?? 0)
                })
                .OrderByDescending(x => x.Stock)
                .ToListAsync();

            return data.Select(x => (x.ModelName, x.Stock)).ToList();
        }

        // 7. Dealer xuất sắc
        public async Task<(string DealerName, int OrdersCount)> GetTopDealerAsync(int storeId)
        {
            var data = await _context.Orders
                .Include(o => o.Dealer)
                .Where(o => o.Status == OrderEnum.Completed && o.Dealer.StoreId == storeId)
                .GroupBy(o => o.Dealer.FullName)
                .Select(g => new
                {
                    DealerName = g.Key,
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.OrdersCount)
                .FirstOrDefaultAsync();

            return data != null ? (data.DealerName, data.OrdersCount) : ("Chưa có dữ liệu", 0);
        }

        // 8. Khách hàng chi tiêu nhiều nhất
        public async Task<(string CustomerName, decimal TotalSpent)> GetTopCustomerAsync(int storeId)
        {
            var data = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Dealer)
                .Where(o => o.Status == OrderEnum.Completed && o.Dealer.StoreId == storeId)
                .GroupBy(o => o.Customer.FullName)
                .Select(g => new
                {
                    CustomerName = g.Key,
                    TotalSpent = g.Sum(x => (decimal?)x.TotalPrice) ?? 0
                })
                .OrderByDescending(x => x.TotalSpent)
                .FirstOrDefaultAsync();

            return data != null ? (data.CustomerName, data.TotalSpent) : ("Chưa có dữ liệu", 0);
        }

        // 9. Tổng số lượng xe bán ra
        public async Task<int> GetTotalVehiclesSoldAsync(int storeId)
        {
            return await _context.Orders
                .Include(o => o.Dealer)
                .Where(o => o.Status == OrderEnum.Completed && o.Dealer.StoreId == storeId)
                .SumAsync(o => (int?)o.Quantity) ?? 0;
        }

        // 10. Xe bán chậm (Left Join từ kho)
        public async Task<List<(string ModelName, int Quantity)>> GetBottomVehiclesAsync(int storeId, int top = 5)
        {
            // B1: Lấy danh sách xe trong kho
            var storeVehicles = await _context.Storages
                .Include(s => s.Vehicle)
                .Where(s => s.StoreId == storeId)
                .Select(s => s.Vehicle.ModelName)
                .Distinct()
                .ToListAsync();

            // B2: Lấy danh sách xe đã bán
            var soldStats = await _context.Quotes
                .Include(q => q.Vehicle)
                .Include(q => q.Dealer)
                .Where(q => q.Status == QuoteEnum.Accepted && q.Dealer.StoreId == storeId)
                .GroupBy(q => q.Vehicle.ModelName)
                .Select(g => new { ModelName = g.Key, Qty = g.Count() })
                .ToListAsync();

            // B3: Left Join để tìm xe bán 0 chiếc
            var result = storeVehicles.GroupJoin(soldStats,
                v => v,
                s => s.ModelName,
                (v, s) => new
                {
                    ModelName = v,
                    Quantity = s.Sum(x => x.Qty)
                })
                .OrderBy(x => x.Quantity)
                .Take(top)
                .ToList();

            return result.Select(x => (x.ModelName, x.Quantity)).ToList();
        }
    }
}