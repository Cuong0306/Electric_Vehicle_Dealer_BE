using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IStoreService
    {
        Task<IEnumerable<StoreResponse>> GetAllStoresAsync();
        Task<StoreResponse> GetStoreByIdAsync(int id);
        Task<StoreResponse> CreateStoreAsync(CreateStoreRequest dto);
        Task<StoreResponse> UpdateStoreAsync(int id, UpdateStoreRequest dto);
        Task<bool> DeleteStoreAsync(int id);
    }
}
