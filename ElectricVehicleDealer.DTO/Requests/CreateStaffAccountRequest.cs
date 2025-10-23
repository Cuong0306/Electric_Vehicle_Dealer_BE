using ElectricVehicleDealer.DAL.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateStaffAccountRequest
    {

        public string FullName { get; set; } = null!;

        public string? Phone { get; set; }

        public string? Email { get; set; }

        public string Password { get; set; } = null!;

        public RoleStaffEnum Role { get; set; } = RoleStaffEnum.EVM_Staff;

        public string? Position { get; set; }

        public string? Status { get; set; }
    }
}
