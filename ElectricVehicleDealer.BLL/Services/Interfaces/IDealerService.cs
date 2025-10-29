using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IDealerService
    {
        Task<IEnumerable<DealerResponse>> GetAllAsync();
        Task<DealerResponse> GetByIdAsync(int id);
        //Task<DealerResponse> CreateAsync(CreateDealerRequest dto);
        Task<DealerResponse> UpdateAsync(int id, UpdateDealerRequest dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteUserAsync(int id);
        Task<bool> HardDeleteUserAsync(int id);
        Task<List<DealerResponse>> GetAllActiveDealerAsync();
    }
}
