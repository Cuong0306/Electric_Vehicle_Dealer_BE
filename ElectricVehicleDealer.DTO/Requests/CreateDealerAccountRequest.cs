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
    public class CreateDealerAccountRequest
    {
        public int? StoreId { get; set; }
        public string? FullName { get; set; }

        public string? Phone { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? Address { get; set; }
        public string? Status { get; set; } 
        public RoleDealerEnum Role { get; set; } = RoleDealerEnum.Dealer_staff;
    }
}
