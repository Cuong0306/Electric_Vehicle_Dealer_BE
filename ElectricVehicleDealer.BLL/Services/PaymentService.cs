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
    public class PaymentService : IPaymentService
    {
        private readonly PaymentRepository _repository;
        public PaymentService(PaymentRepository repository) { _repository = repository; }
        public async Task<int> CreateAsync(CreatePaymentDto payment)
        {

            try
            {
                return await _repository.CreateAsync(payment);
            }
            catch (Exception ex) { }
            return 0;
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

        public async Task<List<Payment>> GetAllAsync()
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

        public async Task<Payment> GetByIdAsync(int id)
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

        public async Task<int> UpdateAsync(Payment payment)
        {
            try
            {
                return await _repository.UpdateAsync(payment);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when updating customer", ex);
            }
        }
    }
}
