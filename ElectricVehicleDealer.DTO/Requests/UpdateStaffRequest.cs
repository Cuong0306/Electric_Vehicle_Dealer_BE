using System;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class UpdateStaffRequest
    {
        public int? BrandId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Status { get; set; }
        public int? StoreId { get; set; }
    }
}
