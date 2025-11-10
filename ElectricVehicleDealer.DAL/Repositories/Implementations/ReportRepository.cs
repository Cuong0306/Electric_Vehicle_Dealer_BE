using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        // Tổng doanh thu từ tất cả các đơn hàng có status = 'Completed'
        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _context.Orders
                .Where(o => o.Status == OrderEnum.Completed)
                .SumAsync(o => (decimal?)o.TotalPrice) ?? 0;
        }

        // Tổng số khách hàng
        public async Task<int> GetTotalCustomersAsync()
        {
            return await _context.Customers.CountAsync();
        }

        // Tổng số đơn hàng
        public async Task<int> GetTotalOrdersAsync()
        {
            return await _context.Orders.CountAsync();
        }

        // Doanh thu theo từng tháng
        public async Task<List<(string Month, decimal Revenue)>> GetRevenueByMonthAsync()
        {
            // 1️⃣ Lấy dữ liệu từ database, chỉ group và sum, không format string
            var data = await _context.Orders
                .Where(o => o.Status == OrderEnum.Completed && o.OrderDate.HasValue)
                .GroupBy(o => new { Year = o.OrderDate.Value.Year, Month = o.OrderDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(x => (decimal?)x.TotalPrice) ?? 0
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(); // <- ToListAsync chuyển dữ liệu sang memory

            // 2️⃣ Format string MM/YYYY trên client-side (memory)
            return data.Select(x => ($"{x.Month:D2}/{x.Year}", x.Revenue)).ToList();
        }



        // Xe bán chạy nhất (dựa vào số lượng trong Orders)
        public async Task<List<(string ModelName, int Quantity)>> GetTopVehiclesAsync(int top = 5)
        {
            
            var data = await _context.Quotes
                .Include(q => q.Vehicle)
                .Include(q => q.Customer)
                .Where(q => q.Status == QuoteEnum.Accepted)
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

        // Tồn kho theo từng model
        public async Task<List<(string ModelName, int Stock)>> GetInventoryAsync()
        {
            var data = await _context.Storages
                .Include(s => s.Vehicle)
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
    }
}