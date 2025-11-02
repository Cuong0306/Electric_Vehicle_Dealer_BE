using System;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class UpdateVehicleRequest
    {
        public int? BrandId { get; set; }
        public string? ModelName { get; set; }
        public string[] ImageUrls { get; set; } = Array.Empty<string>();
        public string? Version { get; set; }
        public int? Year { get; set; }
        public string? Color { get; set; }
        public decimal? Price { get; set; }
        public string? BatteryCapacity { get; set; }
        public string? RangePerCharge { get; set; }
        public string? WarrantyPeriod { get; set; }
        public int? SeatingCapacity { get; set; }
        public string? Transmission { get; set; }
        public int? Airbags { get; set; }
        public int? Horsepower { get; set; }
        public string? VehicleType { get; set; }
        public int? TrunkCapacity { get; set; }
        public int? DailyDrivingLimit { get; set; }
        public string? Screen { get; set; }
        public string? SeatMaterial { get; set; }
        public string? InteriorMaterial { get; set; }
        public string? AirConditioning { get; set; }
        public string? SpeakerSystem { get; set; }
        public string? InVehicleCabinet { get; set; }
        public int? LengthMm { get; set; }
        public int? WidthMm { get; set; }
        public int? HeightMm { get; set; }
        public string? Wheels { get; set; }
        public string? Headlights { get; set; }
        public string? Taillights { get; set; }
        public string? FrameChassis { get; set; }
        public int? DoorCount { get; set; }
        public string? GlassWindows { get; set; }
        public string? Mirrors { get; set; }
        public string? Cameras { get; set; }
        public bool? IsAllocation { get; set; }
        public DateTime? CreateDate { get; set; }
        
    }
}
