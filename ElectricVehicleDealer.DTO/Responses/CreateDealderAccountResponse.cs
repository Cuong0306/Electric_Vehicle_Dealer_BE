using ElectricVehicleDealer.DAL.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class CreateDealderAccountResponse
    {
        public int DealerId { get; set; }

        public string FullName { get; set; } = null!;

        public string? Phone { get; set; }

        public string Password { get; set; } = null!;

        public string? Email { get; set; }

        public string? Address { get; set; }

        public int? StoreId { get; set; }
        public string? Status { get; set; }

        public RoleDealerEnum Role { get; set; } = RoleDealerEnum.Dealer_staff;
    }
}
