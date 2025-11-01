using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly CustomerRepository _repository;
        public CustomerService(CustomerRepository repository) { _repository = repository; }
        public async Task<int> CreateAsync(CreateCustomerDto customer)
        {
            try
            {
                return await _repository.CreateAsync(customer);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw; 
            }
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

        public async Task<List<Customer>> GetAllAsync()
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


        public async Task<Customer> GetByIdAsync(int id)
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

   

        public async Task<List<Customer>> GetCustomersByStoreAsync(int storeId)
        {
            try
            {
                return await _repository.GetCustomersByStoreAsync(storeId);
            }
            catch (Exception ex)
            {
                // có thể log lỗi
                throw new Exception("Error when getting customers by store", ex);
            }
            ;
        }

        public async Task<int> UpdateAsync(Customer customer)
        {
            try
            {
                return await _repository.UpdateAsync(customer);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when updating customer", ex);
            }
        }

    }
}
