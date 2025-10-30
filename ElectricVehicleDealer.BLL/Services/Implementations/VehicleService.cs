using Azure;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces.Implementations
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
                Screen  = dto.Screen,
                SeatMaterial = dto.SeatMaterial,
                InteriorMaterial = dto.InteriorMaterial,
                AirConditioning = dto.AirConditioning,
                SpeakerSystem = dto.SpeakerSystem,
                InVehicleCabinet = dto.InVehicleCabinet,
                LengthMm = dto.LengthMm,
                WidthMm = dto.WidthMm,
                HeightMm = dto.HeightMm,
                Wheels = dto.Wheels,
                Headlights = dto.Headlights,
                Taillights = dto.Taillights,
                FrameChassis = dto.FrameChassis,
                DoorCount = dto.DoorCount,
                GlassWindows = dto.GlassWindows,
                Mirrors = dto.Mirrors,
                Cameras = dto.Cameras,
                IsAllocation = dto.IsAllocation,
                //CreateDate = dto.CreateDate,

            };
            await _unitOfWork.Repository<Vehicle>().AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<VehicleResponse> UpdateAsync(int id, UpdateVehicleRequest dto)
        {
            var entity = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(id);
            if (dto.BrandId != null) entity.BrandId = dto.BrandId.Value;
            if (!string.IsNullOrWhiteSpace(dto.ModelName)) entity.ModelName = dto.ModelName;
            if (!string.IsNullOrWhiteSpace(dto.ImageUrls))
                entity.ImageUrls = dto.ImageUrls.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (!string.IsNullOrWhiteSpace(dto.Version)) entity.Version = dto.Version;
            if (dto.Year != null) entity.Year = dto.Year.Value;
            if (!string.IsNullOrWhiteSpace(dto.Color)) entity.Color = dto.Color;
            if (dto.Price != null) entity.Price = dto.Price.Value;
            if (!string.IsNullOrWhiteSpace(dto.BatteryCapacity))
                entity.BatteryCapacity = dto.BatteryCapacity;

            if (!string.IsNullOrWhiteSpace(dto.RangePerCharge))
                entity.RangePerCharge = dto.RangePerCharge;
            if (!string.IsNullOrWhiteSpace(dto.WarrantyPeriod)) entity.WarrantyPeriod = dto.WarrantyPeriod;
            if (dto.SeatingCapacity != null) entity.SeatingCapacity = dto.SeatingCapacity.Value;
            if (!string.IsNullOrWhiteSpace(dto.Transmission)) entity.Transmission = dto.Transmission;
            if (dto.Airbags != null) entity.Airbags = dto.Airbags.Value;
            if (dto.Horsepower != null) entity.Horsepower = dto.Horsepower.Value;
            if (!string.IsNullOrWhiteSpace(dto.VehicleType)) entity.VehicleType = dto.VehicleType;
            if (dto.TrunkCapacity != null) entity.TrunkCapacity = dto.TrunkCapacity.Value;
            if (dto.DailyDrivingLimit != null) entity.DailyDrivingLimit = dto.DailyDrivingLimit.Value;
            if (!string.IsNullOrWhiteSpace(dto.Screen)) entity.Screen = dto.Screen;
            if (!string.IsNullOrWhiteSpace(dto.SeatMaterial)) entity.SeatMaterial = dto.SeatMaterial;
            if (!string.IsNullOrWhiteSpace(dto.InteriorMaterial)) entity.InteriorMaterial = dto.InteriorMaterial;
            if (!string.IsNullOrWhiteSpace(dto.AirConditioning)) entity.AirConditioning = dto.AirConditioning;
            if (!string.IsNullOrWhiteSpace(dto.SpeakerSystem)) entity.SpeakerSystem = dto.SpeakerSystem;
            if (!string.IsNullOrWhiteSpace(dto.InVehicleCabinet)) entity.InVehicleCabinet = dto.InVehicleCabinet;
            if (dto.LengthMm != null) entity.LengthMm = dto.LengthMm.Value;
            if (dto.WidthMm != null) entity.WidthMm = dto.WidthMm.Value;
            if (dto.HeightMm != null) entity.HeightMm = dto.HeightMm.Value;
            if (!string.IsNullOrWhiteSpace(dto.Wheels)) entity.Wheels = dto.Wheels;
            if (!string.IsNullOrWhiteSpace(dto.Headlights)) entity.Headlights = dto.Headlights;
            if (!string.IsNullOrWhiteSpace(dto.Taillights)) entity.Taillights = dto.Taillights;
            if (!string.IsNullOrWhiteSpace(dto.FrameChassis)) entity.FrameChassis = dto.FrameChassis;
            if (dto.DoorCount != null) entity.DoorCount = dto.DoorCount.Value;
            if (!string.IsNullOrWhiteSpace(dto.GlassWindows)) entity.GlassWindows = dto.GlassWindows;
            if (!string.IsNullOrWhiteSpace(dto.Mirrors)) entity.Mirrors = dto.Mirrors;
            if (!string.IsNullOrWhiteSpace(dto.Cameras)) entity.Cameras = dto.Cameras;
            if (dto.CreateDate != null) entity.CreateDate = dto.CreateDate.Value;
            if (dto.IsAllocation.HasValue)
                entity.IsAllocation = dto.IsAllocation.Value;
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
            Screen = x.Screen,
            SeatMaterial = x.SeatMaterial,
            InteriorMaterial = x.InteriorMaterial,
            AirConditioning = x.AirConditioning,
            SpeakerSystem = x.SpeakerSystem,
            InVehicleCabinet = x.InVehicleCabinet,
            LengthMm = x.LengthMm,
            WidthMm = x.WidthMm,
            HeightMm = x.HeightMm,
            Wheels = x.Wheels,
            Headlights = x.Headlights,
            Taillights = x.Taillights,
            FrameChassis = x.FrameChassis,
            DoorCount = x.DoorCount,
            GlassWindows = x.GlassWindows,
            Mirrors = x.Mirrors,
            Cameras = x.Cameras,
            IsAllocation = x.IsAllocation,
            CreateDate = x.CreateDate,
        };

        public async Task<IEnumerable<VehicleResponse>> GetAllVehicleByStoreIdAsync(int storeId)
        {
            // Truy vấn tất cả các xe theo StoreId
            var vehicles = await _unitOfWork.Vehicles.GetAllByStoreIdAsync(storeId);
            return vehicles.Select(v => MapToResponse(v)).ToList();
        }

        public async Task<IEnumerable<VehicleResponse>> GetAllVehicleByBrandIdAsync(int brandId)
        {
            var vehicles = await _unitOfWork.Vehicles.GetAllByBrandIdAsync(brandId);
            return vehicles.Select(v => MapToResponse(v)).ToList();
        }
    }
}
