using System;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateDealerRequest
    {
        public string FullName { get; set; }

        public string Password{ get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? StoreId { get; set; }
    }
}
