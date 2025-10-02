using System;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class StorageResponse
    {
        public int StorageId { get; set; }
        public int VehicleId { get; set; }
        public int StoreId { get; set; }
        public int? QuantityAvailable { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
