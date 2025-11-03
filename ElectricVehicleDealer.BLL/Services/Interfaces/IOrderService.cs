using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IOrderService
    {
        Task<List<OrderResponse>> GetAllAsync();
        Task<OrderResponse> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateOrderDto order);
        Task<UpdateOrderResponse?> UpdateOrderAsync(int id, UpdateOrderRequest request);
        Task<bool> DeleteAsync(int id);

        Task<List<OrderResponse>> GetOrdersByBrandIdAsync(int brandId);

    }
}
