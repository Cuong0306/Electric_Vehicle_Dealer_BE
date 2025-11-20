using ElectricVehicleDealer.BLL.Extensions;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
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
            
            if (await IsEmailDuplicateAsync(customer.Email))
            {
                throw new Exception($"Email '{customer.Email}' đã tồn tại trong hệ thống.");
            }

            try
            {
                return await _repository.CreateAsync(customer);
            }
            catch (Exception ex)
            {
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

        public async Task<List<GetAllCustomerResponse>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error when getting all customers", ex);
            }
        }


        public async Task<GetAllCustomerResponse> GetByIdAsync(int id)
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



        public async Task<List<GetAllCustomerResponse>> GetCustomersByStoreAsync(int storeId)
        {
            try
            {
                return await _repository.GetCustomersByStoreAsync(storeId);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when getting customers by store", ex);
            }
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

        public async Task<PagedResult<GetAllCustomerResponse>> GetPagedAsync(
            int pageNumber, int pageSize, string? search = null, string? sortBy = null, string? status = null)
        {
            var query = _repository.GetAllCustomerQuery();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c =>
                    c.FullName.Contains(search) ||
                    c.Email.Contains(search) ||
                    c.Phone.Contains(search));
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(c => c.Status.ToString() == status);
            }

            query = sortBy?.ToLower() switch
            {
                "fullname" => query.OrderBy(c => c.FullName),
                "email" => query.OrderBy(c => c.Email),
                "createdate" => query.OrderByDescending(c => c.CreateDate),
                _ => query.OrderBy(c => c.CustomerId)
            };
            return await query.ToPagedResultAsync(pageNumber, pageSize);
        }

        public async Task<bool> IsEmailDuplicateAsync(string email, int? excludeCustomerId = null)
        {
            var query = _repository.GetAllCustomerQuery()
                .Where(c => c.Email.ToLower() == email.Trim().ToLower());

            if (excludeCustomerId.HasValue)
            {
                query = query.Where(c => c.CustomerId != excludeCustomerId.Value);
            }

            return await query.AnyAsync();
        }

    }

}

