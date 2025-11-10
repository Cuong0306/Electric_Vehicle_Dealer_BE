using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IReportRepository _reportRepository;

        public ReportService(IReportRepository reportRepository)
        {
            _reportRepository = reportRepository;
        }

        public async Task<decimal> GetTotalRevenueAsync(int storeId)
        {
            return await _reportRepository.GetTotalRevenueAsync(storeId);
        }

        public async Task<int> GetTotalCustomersAsync(int storeId)
        {
            return await _reportRepository.GetTotalCustomersAsync(storeId);
        }

        public async Task<int> GetTotalOrdersAsync(int storeId)
        {
            return await _reportRepository.GetTotalOrdersAsync(storeId);
        }

        public async Task<List<(string Month, decimal Revenue)>> GetRevenueByMonthAsync(int storeId)
        {
            return await _reportRepository.GetRevenueByMonthAsync(storeId);
        }

        public async Task<List<(string ModelName, int Quantity)>> GetTopVehiclesAsync(int storeId, int top = 5)
        {
            return await _reportRepository.GetTopVehiclesAsync(storeId, top);
        }

        public async Task<List<(string ModelName, int Stock)>> GetInventoryAsync(int storeId)
        {
            return await _reportRepository.GetInventoryAsync(storeId);
        }

        public async Task<(string DealerName, int OrdersCount)> GetTopDealerAsync(int storeId)
        {
            return await _reportRepository.GetTopDealerAsync(storeId);
        }

        public async Task<(string CustomerName, decimal TotalSpent)> GetTopCustomerAsync(int storeId)
        {
            return await _reportRepository.GetTopCustomerAsync(storeId);
        }

        public async Task<int> GetTotalVehiclesSoldAsync(int storeId)
        {
            return await _reportRepository.GetTotalVehiclesSoldAsync(storeId);
        }

        public async Task<List<(string ModelName, int Quantity)>> GetBottomVehiclesAsync(int storeId, int top = 5)
        {
            return await _reportRepository.GetBottomVehiclesAsync(storeId, top);
        }

    }
}
