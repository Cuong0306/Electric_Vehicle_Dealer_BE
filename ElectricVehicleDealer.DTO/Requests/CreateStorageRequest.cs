using System;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateStorageRequest
    {
        public int VehicleId { get; set; }
        public int StoreId { get; set; }
        public int? QuantityAvailable { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
