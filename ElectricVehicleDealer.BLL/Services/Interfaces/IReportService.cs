using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IReportService
    {
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalCustomersAsync();
        Task<int> GetTotalOrdersAsync();
        Task<List<(string Month, decimal Revenue)>> GetRevenueByMonthAsync();
        Task<List<(string ModelName, int Quantity)>> GetTopVehiclesAsync(int top = 5);
        Task<List<(string ModelName, int Stock)>> GetInventoryAsync();
    }
}
