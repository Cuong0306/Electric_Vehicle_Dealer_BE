using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<VehicleResponse>> GetAllAsync();
        Task<VehicleResponse> GetByIdAsync(int id);
        Task<VehicleResponse> CreateAsync(CreateVehicleRequest dto);
        Task<VehicleResponse> UpdateAsync(int id, UpdateVehicleRequest dto);
        Task<bool> DeleteAsync(int id);
    }
}
