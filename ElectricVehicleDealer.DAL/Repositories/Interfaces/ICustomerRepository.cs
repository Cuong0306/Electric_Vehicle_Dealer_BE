using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface ICustomerRepository
    {
        //Task<List<GetAllCustomerResponse>> GetAllAsync();
        //Task<Customer?> GetByIdAsync(int id);
        //Task<int> CreateAsync(CreateCustomerDto dto);
        //Task<Customer?> UpdateAsync(UpdateCustomerRequest request, int id);
        //Task<Customer> UpdateCustomerAsync(Customer customer);
        //Task<List<Customer>> GetAllActiveCustomersAsync();
        //Task<List<Customer>> GetCustomersByStoreAsync(int storeId);
        //Task<bool> HardDeleteCustomerAsync(int id);
        public interface ICustomerRepository
        {
            Task<PagedResult<GetAllCustomerResponse>> GetPagedAsync(
                int pageNumber,
                int pageSize,
                string? search = null,
                string? sortBy = null,
                string? status = null);
        }
    }
}
