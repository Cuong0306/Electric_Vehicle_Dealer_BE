using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class GetAllCustomerResponse
    {
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public string? LicenseUp { get; set; }
        public string? LicenseDown { get; set; }
        public DateTime? CreateDate { get; set; }

        public List<AgreementResponse> Agreements { get; set; } = new();
        public List<OrderLiteResponse> Orders { get; set; } = new();
    }
}
