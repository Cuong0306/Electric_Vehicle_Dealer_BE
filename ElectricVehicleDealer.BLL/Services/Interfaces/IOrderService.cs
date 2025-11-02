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
        Task<Order> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateOrderDto order);
        Task<int> UpdateAsync(Order order);
        Task<bool> DeleteAsync(int id);

    }
}
