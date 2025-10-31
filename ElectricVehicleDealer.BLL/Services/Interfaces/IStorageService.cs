using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IStorageService
    {
        Task<IEnumerable<StorageResponse>> GetAllAsync();
        Task<StorageResponse> GetByIdAsync(int id);
        Task<StorageResponse> CreateAsync(CreateStorageRequest dto);
        Task<StorageResponse> UpdateAsync(int id, UpdateStorageRequest dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> AllocateVehiclesAsync(AllocateVehicleDto dto);
        Task<IEnumerable<VehicleResponse>> GetVehiclesByStoreIdAsync(int storeId);
        Task<bool> RecallVehiclesAsync(AllocateVehicleDto dto);
        Task<IEnumerable<StorageResponse>> GetStorageByBrandIdAsync(int brandId);

        Task<IEnumerable<StorageResponse>> GetByFilterAsync(int? brandId, int? vehicleId);

    }
}
