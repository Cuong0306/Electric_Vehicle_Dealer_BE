using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories;
using ElectricVehicleDealer.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly OrderRepository _repository;
        public OrderService(OrderRepository repository) { _repository = repository; }
        public async Task<int> CreateAsync(CreateOrderDto order)
        {
            return await _repository.CreateAsync(order); 
        }


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var item = await _repository.GetByIdAsync(id);

                if (item != null)
                {
                    return await _repository.DeleteAsync(id);
                }
            }
            catch (Exception ex) { }
            return false;
        }

        public async Task<List<Order>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                // có thể log lỗi
                throw new Exception("Error when getting all customers", ex);
            }
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when getting customer by id", ex);
            }
        }

        public async Task<int> UpdateAsync(Order order)
        {
            try
            {
                return await _repository.UpdateAsync(order);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when updating customer", ex);
            }
        }
    }
}
