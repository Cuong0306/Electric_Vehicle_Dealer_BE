using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IQuoteService
    {
        Task<IEnumerable<QuoteResponse>> GetQuotesByCustomerIdAsync(int customerId);
        Task<IEnumerable<QuoteResponse>> GetAllAsync();
        Task<QuoteResponse> GetByIdAsync(int id);
        Task<QuoteResponse> CreateAsync(CreateQuoteRequest dto);
        Task<QuoteResponse> UpdateAsync(int id, UpdateQuoteRequest dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<CustomerResponse>> GetCustomersWithQuotesAsync();
    }
}
