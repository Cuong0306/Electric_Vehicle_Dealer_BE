using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface ICustomerRepository : IGenericRepository<Customer>
    {
        
        Task<Customer?> GetByEmailAsync(string email);
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
