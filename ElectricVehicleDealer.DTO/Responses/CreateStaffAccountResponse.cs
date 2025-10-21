using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class CreateStaffAccountResponse
    {
        public int StaffId { get; set; }
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string Password { get; set; } = null!;
        public int Role { get; set; }
        public string? Position { get; set; }
        public string? Status { get; set; }
    }
}
