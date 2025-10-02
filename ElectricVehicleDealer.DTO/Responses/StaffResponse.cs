using System;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class StaffResponse
    {
        public int StaffId { get; set; }
        public string FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public int? StoreId { get; set; }
    }
}
