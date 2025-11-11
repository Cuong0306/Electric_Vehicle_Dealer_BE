using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<List<OrderResponse>> GetAllAsync();
        Task<OrderResponse?> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateOrderDto dto);
        Task<int> UpdateAsync(Order order);
        Task<bool> DeleteAsync(int id);
        Task<Order?> GetEntityByIdAsync(int id);
        Task<List<OrderResponse>> GetOrdersByBrandIdAsync(int brandId);
        Task<List<OrderResponse>> GetOrdersByStoreIdAsync(int storeId);
        IQueryable<Order> GetOrderQuery();
    }
}
