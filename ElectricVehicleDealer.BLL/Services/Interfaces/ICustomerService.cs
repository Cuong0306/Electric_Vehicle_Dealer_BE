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
    public interface ICustomerService
    {
        Task<List<GetAllCustomerResponse>> GetAllAsync();
        Task<GetAllCustomerResponse> GetByIdAsync(int id);

        Task<List<Customer>> GetCustomersByStoreAsync(int storeId);

        Task<int> CreateAsync(CreateCustomerDto customer);
        Task<int> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<GetAllCustomerResponse>> GetPagedAsync(
            int pageNumber,
            int pageSize,
            string? search,
            string? sortBy,
            string? status
        );
    }
}
