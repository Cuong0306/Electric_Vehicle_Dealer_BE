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

        public async Task<bool> AllocateVehiclesAsync(AllocateVehicleDto dto)
        {
            if (dto.VehicleIds == null || !dto.VehicleIds.Any())
                throw new ArgumentException("Danh sách VehicleIds không được để trống.");

            foreach (var vehicleId in dto.VehicleIds)
            {
                // Lấy xe theo Id
                var vehicle = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(vehicleId);
                if (vehicle == null)
                    continue; // bỏ qua xe không tồn tại

                // Đánh dấu đã phân bổ
                vehicle.IsAllocation = true;
                _unitOfWork.Repository<Vehicle>().Update(vehicle);

                // Tạo bản ghi trong Storage
                var storage = new Storage
                {
                    VehicleId = vehicleId,
                    StoreId = dto.StoreId,
                    QuantityAvailable = 1,
                    LastUpdated = DateTime.Now
                };

                await _unitOfWork.Repository<Storage>().AddAsync(storage);
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<bool> RecallVehiclesAsync(AllocateVehicleDto dto)
        {
            if (dto.VehicleIds == null || !dto.VehicleIds.Any())
                throw new ArgumentException("Danh sách VehicleIds không được để trống.");

            foreach (var vehicleId in dto.VehicleIds)
            {
                // --- 1. Cập nhật lại Vehicle ---
                var vehicle = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(vehicleId);
                if (vehicle == null)
                    continue;

                vehicle.IsAllocation = false; // Đánh dấu là chưa phân bổ
                _unitOfWork.Repository<Vehicle>().Update(vehicle);

                // --- 2. Xóa bản ghi trong Storage của storeId ---
                var storages = await _unitOfWork.Repository<Storage>().GetAllAsync();
                var storageRecord = storages.FirstOrDefault(s =>
                    s.VehicleId == vehicleId && s.StoreId == dto.StoreId);

                if (storageRecord != null)
                {
                    _unitOfWork.Repository<Storage>().Remove(storageRecord);
                }
            }

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<VehicleResponse>> GetVehiclesByStoreIdAsync(int storeId)
        {
            // Lấy danh sách Storage có storeId tương ứng
            var storages = await _unitOfWork.Repository<Storage>().GetAllAsync();
            var vehiclesInStore = storages.Where(s => s.StoreId == storeId).ToList();

            if (!vehiclesInStore.Any())
                return Enumerable.Empty<VehicleResponse>();

            // Lấy danh sách vehicleId từ Storage
            var vehicleIds = vehiclesInStore.Select(s => s.VehicleId).Distinct().ToList();

            // Lấy danh sách Vehicle tương ứng
            var vehicles = await _unitOfWork.Repository<Vehicle>().GetAllAsync();
            var filtered = vehicles.Where(v => vehicleIds.Contains(v.VehicleId));

            // Map sang DTO response
            return filtered.Select(v => new VehicleResponse
            {
                VehicleId = v.VehicleId,
                ModelName = v.ModelName,
                Color = v.Color,
                Price = v.Price,
                Year = v.Year,
                VehicleType = v.VehicleType,
                CreateDate = v.CreateDate
            });
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
