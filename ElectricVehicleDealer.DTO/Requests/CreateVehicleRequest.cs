using System;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateVehicleRequest
    {
        public int BrandId { get; set; }
        public string ModelName { get; set; }
        public string? Version { get; set; }
        public int? Year { get; set; }
        public string? Color { get; set; }
        public decimal? Price { get; set; }
        public string? BatteryCapacity { get; set; }
        public string? RangePerCharge { get; set; }
        public string? WarrantyPeriod { get; set; }
        public string[] ImageUrls { get; set; } = Array.Empty<string>();
        public int? SeatingCapacity { get; set; }           // 4 seats
        public string? Transmission { get; set; }          // Automatic
        public int? Airbags { get; set; }                  // 1 airbag
        public int? Horsepower { get; set; }               // 43 HP
        public string? VehicleType { get; set; }           // Minicar
        public int? TrunkCapacity { get; set; }            // 285 L
        public int? DailyDrivingLimit { get; set; }        // 300 km/day
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
        public bool IsAllocation { get; set; } = false;
        //public DateTime? CreateDate { get; set; }
    }
}
