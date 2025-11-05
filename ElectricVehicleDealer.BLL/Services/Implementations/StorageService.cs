using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces.Implementations
{
    public class StorageService : IStorageService
    {
        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        public StorageService(IUnitOfWork uow) => _unitOfWork = uow;

        public async Task<IEnumerable<StorageResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<Storage>().GetAllAsync();
            return list.Select(x => MapToResponse(x));
        }

        public async Task<bool> AllocateVehiclesAsync(AllocateVehicleDto dto)
        {
            if (dto.Stores == null || !dto.Stores.Any())
                throw new ArgumentException("Danh sách store không được để trống.");

            var vehicle = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(dto.VehicleId);
            if (vehicle == null)
                throw new ArgumentException("Xe không tồn tại.");

            // 1️⃣ Lấy kho tổng của xe này
            var mainStorage = (await _unitOfWork.Repository<Storage>().GetAllAsync())
                .FirstOrDefault(s => s.VehicleId == dto.VehicleId && s.StoreId == null);

            if (mainStorage == null)
                throw new InvalidOperationException("Không tìm thấy kho tổng cho xe này.");

            // 2️⃣ Duyệt từng store được phân bổ
            foreach (var storeInfo in dto.Stores)
            {
                // Kiểm tra số lượng hợp lệ
                if (storeInfo.Quantity <= 0)
                    throw new ArgumentException($"Số lượng phải > 0 (StoreId: {storeInfo.StoreId})");

                // Kiểm tra đủ hàng trong kho tổng
                if (mainStorage.QuantityAvailable < storeInfo.Quantity)
                    throw new InvalidOperationException(
                        $"Kho tổng không đủ xe để phân bổ (còn {mainStorage.QuantityAvailable}, yêu cầu {storeInfo.Quantity}).");

                // ➖ Trừ trong kho tổng
                mainStorage.QuantityAvailable -= storeInfo.Quantity;
                mainStorage.LastUpdated = DateTime.Now;
                _unitOfWork.Repository<Storage>().Update(mainStorage);

                // ➕ Cộng vào kho chi nhánh
                var existingStorage = (await _unitOfWork.Repository<Storage>().GetAllAsync())
                    .FirstOrDefault(s => s.VehicleId == dto.VehicleId && s.StoreId == storeInfo.StoreId);

                if (existingStorage != null)
                {
                    existingStorage.QuantityAvailable += storeInfo.Quantity;
                    existingStorage.LastUpdated = DateTime.Now;
                    _unitOfWork.Repository<Storage>().Update(existingStorage);
                }
                else
                {
                    var newStorage = new Storage
                    {
                        VehicleId = dto.VehicleId,
                        StoreId = storeInfo.StoreId,
                        QuantityAvailable = storeInfo.Quantity,
                        LastUpdated = DateTime.Now,
                        BrandId = vehicle.BrandId
                    };
                    await _unitOfWork.Repository<Storage>().AddAsync(newStorage);
                }
            }

            await _unitOfWork.SaveAsync();
            return true;
        }



        public async Task<bool> RecallVehiclesAsync(AllocateVehicleDto dto)
        {
            if (dto.Stores == null || !dto.Stores.Any())
                throw new ArgumentException("Danh sách store không được để trống.");

            var vehicle = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(dto.VehicleId);
            if (vehicle == null)
                throw new ArgumentException("Xe không tồn tại.");

            foreach (var storeInfo in dto.Stores)
            {
                var existingStorage = (await _unitOfWork.Repository<Storage>().GetAllAsync())
                    .FirstOrDefault(s => s.VehicleId == dto.VehicleId && s.StoreId == storeInfo.StoreId);

                if (existingStorage != null)
                {
                    // Trừ số lượng
                    existingStorage.QuantityAvailable -= storeInfo.Quantity;

                    if (existingStorage.QuantityAvailable <= 0)
                    {
                        // Nếu hết xe thì xóa luôn dòng storage
                        _unitOfWork.Repository<Storage>().Remove(existingStorage);
                    }
                    else
                    {
                        existingStorage.LastUpdated = DateTime.Now;
                        _unitOfWork.Repository<Storage>().Update(existingStorage);
                    }
                }
                // Nếu không có record thì bỏ qua (vì không thể thu hồi từ store chưa có xe)
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
            var existingStorage = (await _unitOfWork.Repository<Storage>().GetAllAsync())
                .FirstOrDefault(s =>
                    s.VehicleId == dto.VehicleId &&
                    s.BrandId == dto.BrandId &&
                    s.StoreId == dto.StoreId);

            if (existingStorage != null)
            {
                existingStorage.QuantityAvailable += dto.QuantityAvailable;
                existingStorage.LastUpdated = DateTime.Now;
                _unitOfWork.Repository<Storage>().Update(existingStorage);
                await _unitOfWork.SaveAsync();
                return MapToResponse(existingStorage);
            }

            var newStorage = new Storage
            {
                VehicleId = dto.VehicleId,
                BrandId = dto.BrandId,
                StoreId = dto.StoreId,
                QuantityAvailable = dto.QuantityAvailable,
                LastUpdated = DateTime.Now
            };

            await _unitOfWork.Repository<Storage>().AddAsync(newStorage);
            await _unitOfWork.SaveAsync();

            return MapToResponse(newStorage);
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
            BrandId = x.BrandId,
            QuantityAvailable = x.QuantityAvailable,
            LastUpdated = x.LastUpdated,
        };
        public async Task<IEnumerable<StorageResponse>> GetStorageByBrandIdAsync(int brandId)
        {
            var storages = await _unitOfWork.Repository<Storage>()
                .GetAllAsync();

            var storagesByBrand = storages
                .Join(_unitOfWork.Repository<Vehicle>().GetAllAsync().Result,
                      s => s.VehicleId,
                      v => v.VehicleId,
                      (s, v) => new { Storage = s, Vehicle = v })
                .Where(x => x.Vehicle != null && x.Vehicle.BrandId == brandId)
                .Select(x => x.Storage)
                .ToList();

            if (!storagesByBrand.Any())
            {
                return Enumerable.Empty<StorageResponse>();
            }

            return storagesByBrand.Select(MapToResponse);
        }

        public async Task<IEnumerable<StorageResponse>> GetByFilterAsync(int? brandId, int? vehicleId)
        {
            var allStorages = await _unitOfWork.Repository<Storage>().GetAllAsync();
            var query = allStorages.AsQueryable();
            if (brandId != 0)
            {
                query = query.Where(b => b.BrandId == brandId);
            }
            if (vehicleId != 0)
            {
                query = query.Where(v => v.VehicleId == vehicleId);
            }
            var storageEntities = query.ToList();
            return storageEntities.Select(MapToResponse);
        }
    }
}
