using System;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class VehicleResponse
    {
        public int VehicleId { get; set; }
        public int BrandId { get; set; }
        public string ModelName { get; set; }
        public string? Version { get; set; }
        public int? Year { get; set; }
        public string? Color { get; set; }
        public decimal? Price { get; set; }
        public string[] ImageUrls { get; set; } = Array.Empty<string>();
        public string? BatteryCapacity { get; set; }
        public string? RangePerCharge { get; set; }
        public string? WarrantyPeriod { get; set; }
        public int? SeatingCapacity { get; set; }           // 4 seats
        public string? Transmission { get; set; }          // Automatic
        public int? Airbags { get; set; }                  // 1 airbag
        public int? Horsepower { get; set; }               // 43 HP
        public string? VehicleType { get; set; }           // Minicar
        public int? TrunkCapacity { get; set; }            // 285 L
        public int? DailyDrivingLimit { get; set; }        // 300 km/day
        public DateTime? CreateDate { get; set; }
    }
}
