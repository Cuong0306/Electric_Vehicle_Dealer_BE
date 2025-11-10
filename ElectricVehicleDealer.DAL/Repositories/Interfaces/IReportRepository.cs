using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IReportRepository
    {
        Task<decimal> GetTotalRevenueAsync(int storeId);
        Task<int> GetTotalCustomersAsync(int storeId);
        Task<int> GetTotalOrdersAsync(int storeId);
        Task<List<(string Month, decimal Revenue)>> GetRevenueByMonthAsync(int storeId);
        Task<List<(string ModelName, int Quantity)>> GetTopVehiclesAsync(int storeId, int top = 5);
        Task<List<(string ModelName, int Stock)>> GetInventoryAsync(int storeId);
        Task<(string DealerName, int OrdersCount)> GetTopDealerAsync(int storeId);
        Task<(string CustomerName, decimal TotalSpent)> GetTopCustomerAsync(int storeId);
        Task<int> GetTotalVehiclesSoldAsync(int storeId);
        Task<List<(string ModelName, int Quantity)>> GetBottomVehiclesAsync(int storeId, int top = 5);


    }
}
