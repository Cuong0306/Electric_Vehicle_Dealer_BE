using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateCustomerDto
    {
        public string FullName { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? LicenseUp { get; set; }
        public string? LicenseDown { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }

        public int StoreId { get; set; }
    }
}
