using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _reportRepository.GetTotalRevenueAsync();
        }

        public async Task<int> GetTotalCustomersAsync()
        {
            return await _reportRepository.GetTotalCustomersAsync();
        }

        public async Task<int> GetTotalOrdersAsync()
        {
            return await _reportRepository.GetTotalOrdersAsync();
        }

        public async Task<List<(string Month, decimal Revenue)>> GetRevenueByMonthAsync()
        {
            return await _reportRepository.GetRevenueByMonthAsync();
        }

        public async Task<List<(string ModelName, int Quantity)>> GetTopVehiclesAsync(int top = 5)
        {
            return await _reportRepository.GetTopVehiclesAsync(top);
        }

        public async Task<List<(string ModelName, int Stock)>> GetInventoryAsync()
        {
            return await _reportRepository.GetInventoryAsync();
        }
    }
}
