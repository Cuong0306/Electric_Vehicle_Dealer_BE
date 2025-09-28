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
        public string? BatteryCapacity { get; set; }
        public string? RangePerCharge { get; set; }
        public string? WarrantyPeriod { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
