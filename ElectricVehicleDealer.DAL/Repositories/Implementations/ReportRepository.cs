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

        // Tổng doanh thu theo storeId
        public async Task<decimal> GetTotalRevenueAsync(int storeId)
        {
            return await (from o in _context.Orders
                          join d in _context.Dealers on o.DealerId equals d.DealerId
                          where o.Status == OrderEnum.Completed && d.StoreId == storeId
                          select (decimal?)o.TotalPrice)
                          .SumAsync() ?? 0;
        }

        // Tổng số khách hàng theo storeId
        public async Task<int> GetTotalCustomersAsync(int storeId)
        {
            return await _context.StoreCustomers
                .Where(sc => sc.StoreId == storeId)
                .Select(sc => sc.CustomerId)
                .Distinct()
                .CountAsync();
        }

        // Tổng số đơn hàng theo storeId
        public async Task<int> GetTotalOrdersAsync(int storeId)
        {
            return await _context.Orders
                .Join(_context.Dealers,
                      o => o.DealerId,
                      d => d.DealerId,
                      (o, d) => new { o, d })
                .Where(x => x.o.Status == OrderEnum.Completed && x.d.StoreId == storeId)
                .CountAsync();
        }

        // Doanh thu theo tháng theo storeId
        public async Task<List<(string Month, decimal Revenue)>> GetRevenueByMonthAsync(int storeId)
        {
            var data = await (from o in _context.Orders
                              join d in _context.Dealers on o.DealerId equals d.DealerId
                              where o.Status == OrderEnum.Completed
                                    && o.OrderDate.HasValue
                                    && d.StoreId == storeId
                              group o by new { o.OrderDate.Value.Year, o.OrderDate.Value.Month } into g
                              select new
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

        // Xe bán chạy nhất theo storeId
        public async Task<List<(string ModelName, int Quantity)>> GetTopVehiclesAsync(int storeId, int top = 5)
        {
            var data = await _context.Quotes
                .Include(q => q.Vehicle)
                .Include(q => q.Dealer)
                .Where(q => q.Status == QuoteEnum.Accepted && q.Dealer.StoreId == storeId)
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

        // Tồn kho theo model và storeId
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

        // Dealer thực hiện nhiều order nhất theo storeId
        public async Task<(string DealerName, int OrdersCount)> GetTopDealerAsync(int storeId)
        {
            var data = await _context.Orders
                .Join(_context.Dealers, o => o.DealerId, d => d.DealerId, (o, d) => new { o, d })
                .Where(x => x.o.Status == OrderEnum.Completed && x.d.StoreId == storeId)
                .GroupBy(x => x.d.FullName)
                .Select(g => new
                {
                    DealerName = g.Key,
                    OrdersCount = g.Count()
                })
                .OrderByDescending(x => x.OrdersCount)
                .FirstOrDefaultAsync();

            return data != null ? (data.DealerName, data.OrdersCount) : ("N/A", 0);
        }

        // Customer chi tiêu nhiều nhất theo storeId
        public async Task<(string CustomerName, decimal TotalSpent)> GetTopCustomerAsync(int storeId)
        {
            var data = await _context.Orders
                .Join(_context.Dealers, o => o.DealerId, d => d.DealerId, (o, d) => new { o, d })
                .Join(_context.Customers, od => od.o.CustomerId, c => c.CustomerId, (od, c) => new { od.o, od.d, c })
                .Where(x => x.o.Status == OrderEnum.Completed && x.d.StoreId == storeId)
                .GroupBy(x => x.c.FullName)
                .Select(g => new
                {
                    CustomerName = g.Key,
                    TotalSpent = g.Sum(x => (decimal?)x.o.TotalPrice) ?? 0
                })
                .OrderByDescending(x => x.TotalSpent)
                .FirstOrDefaultAsync();

            return data != null ? (data.CustomerName, data.TotalSpent) : ("N/A", 0);
        }

        // Tổng số lượng xe bán ra theo storeId
        public async Task<int> GetTotalVehiclesSoldAsync(int storeId)
        {
            return await _context.Orders
                .Join(_context.Dealers, o => o.DealerId, d => d.DealerId, (o, d) => new { o, d })
                .Where(x => x.o.Status == OrderEnum.Completed && x.d.StoreId == storeId)
                .SumAsync(x => (int?)x.o.Quantity) ?? 0;
        }

        // Top 5 xe ít được mua theo storeId
        public async Task<List<(string ModelName, int Quantity)>> GetBottomVehiclesAsync(int storeId, int top = 5)
        {
            var data = await _context.Quotes
                .Include(q => q.Vehicle)
                .Include(q => q.Dealer)
                .Where(q => q.Status == QuoteEnum.Accepted && q.Dealer.StoreId == storeId)
                .GroupBy(q => q.Vehicle.ModelName)
                .Select(g => new
                {
                    ModelName = g.Key,
                    Quantity = g.Count()
                })
                .OrderBy(x => x.Quantity) // ít nhất lên trước
                .Take(top)
                .ToListAsync();

            return data.Select(x => (x.ModelName, x.Quantity)).ToList();
        }

    }
}
