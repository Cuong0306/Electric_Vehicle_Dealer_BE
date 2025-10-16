using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class VehicleService : IVehicleService
    {
        private readonly IUnitOfWork _unitOfWork;
        public VehicleService(IUnitOfWork uow) => _unitOfWork = uow;

        public async Task<IEnumerable<VehicleResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<Vehicle>().GetAllAsync();
            return list.Select(x => MapToResponse(x));
        }

        public async Task<VehicleResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(id);
            return MapToResponse(entity);
        }

        public async Task<VehicleResponse> CreateAsync(CreateVehicleRequest dto)
        {
            var entity = new Vehicle()
            {
                BrandId = dto.BrandId,
                ModelName = dto.ModelName,
                Version = dto.Version,
                Year = dto.Year,
                Color = dto.Color,
                Price = dto.Price,
                BatteryCapacity = dto.BatteryCapacity,
                RangePerCharge = dto.RangePerCharge,
                WarrantyPeriod = dto.WarrantyPeriod,
                DailyDrivingLimit = dto.DailyDrivingLimit,
                TrunkCapacity = dto.TrunkCapacity,
                VehicleType = dto.VehicleType,
                Horsepower = dto.Horsepower,
                Airbags = dto.Airbags,
                Transmission = dto.Transmission,
                SeatingCapacity = dto.SeatingCapacity,
                ImageUrls = dto.ImageUrls,
                CreateDate = dto.CreateDate,
            };
            await _unitOfWork.Repository<Vehicle>().AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<VehicleResponse> UpdateAsync(int id, UpdateVehicleRequest dto)
        {
            var entity = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(id);
            entity.BrandId = dto.BrandId;
            entity.ModelName = dto.ModelName;
            entity.Version = dto.Version;
            entity.Year = dto.Year;
            entity.Color = dto.Color;
            entity.Price = dto.Price;
            entity.BatteryCapacity = dto.BatteryCapacity;
            entity.RangePerCharge = dto.RangePerCharge;
            entity.WarrantyPeriod = dto.WarrantyPeriod;
            entity.CreateDate = dto.CreateDate;
            _unitOfWork.Repository<Vehicle>().Update(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(id);
            _unitOfWork.Repository<Vehicle>().Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static VehicleResponse MapToResponse(Vehicle x) => new VehicleResponse
        {
            VehicleId = x.VehicleId,
            BrandId = x.BrandId,
            ModelName = x.ModelName,
            Version = x.Version,
            Year = x.Year,
            Color = x.Color,
            Price = x.Price,
            BatteryCapacity = x.BatteryCapacity,
            RangePerCharge = x.RangePerCharge,
            WarrantyPeriod = x.WarrantyPeriod,
            DailyDrivingLimit = x.DailyDrivingLimit,
            TrunkCapacity = x.TrunkCapacity,
            VehicleType = x.VehicleType,
            Horsepower = x.Horsepower,
            Airbags = x.Airbags,
            Transmission = x.Transmission,
            SeatingCapacity = x.SeatingCapacity,
            ImageUrls = x.ImageUrls,
            CreateDate = x.CreateDate,
        };
    }
}
