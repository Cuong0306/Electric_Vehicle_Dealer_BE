using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces.Implementations
{
    public class StorageService : IStorageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StorageService(IUnitOfWork uow) => _unitOfWork = uow;

        public async Task<IEnumerable<StorageResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<Storage>().GetAllAsync();
            return list.Select(x => MapToResponse(x));
        }

        public async Task<StorageResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Storage>().GetByIdAsync(id);
            return MapToResponse(entity);
        }

        public async Task<StorageResponse> CreateAsync(CreateStorageRequest dto)
        {
            var entity = new Storage()
            {
                VehicleId = dto.VehicleId,
                StoreId = dto.StoreId,
                QuantityAvailable = dto.QuantityAvailable,
                LastUpdated = dto.LastUpdated,
            };
            await _unitOfWork.Repository<Storage>().AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<StorageResponse> UpdateAsync(int id, UpdateStorageRequest dto)
        {
            var entity = await _unitOfWork.Repository<Storage>().GetByIdAsync(id);
            entity.VehicleId = dto.VehicleId;
            entity.StoreId = dto.StoreId;
            entity.QuantityAvailable = dto.QuantityAvailable;
            entity.LastUpdated = dto.LastUpdated;
            _unitOfWork.Repository<Storage>().Update(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Storage>().GetByIdAsync(id);
            _unitOfWork.Repository<Storage>().Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static StorageResponse MapToResponse(Storage x) => new StorageResponse
        {
            StorageId = x.StorageId,
            VehicleId = x.VehicleId,
            StoreId = x.StoreId,
            QuantityAvailable = x.QuantityAvailable,
            LastUpdated = x.LastUpdated,
        };
    }
}
