using ElectricVehicleDealer.DAL.Enum;
using System;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class DealerResponse
    {
        public int DealerId { get; set; }
        public string FullName { get; set; }

        public RoleDealerEnum Role { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public int? StoreId { get; set; }
    }
}
