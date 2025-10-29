using ElectricVehicleDealer.DAL.Enum;
using System;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class StaffResponse
    {
        public int StaffId { get; set; }
        public int? BrandId { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Status { get; set; }
        public string? Position { get; set; }
        public RoleStaffEnum? Role
        { get; set; }


    }
}
