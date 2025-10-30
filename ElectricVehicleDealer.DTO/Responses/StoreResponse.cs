using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class StoreResponse
    {
        public int? StoreId { get; set; }

        public string? StoreName { get; set; }

        public string? Address { get; set; }

        public string? Email { get; set; }

        public int? PromotionId { get; set; }
    }
}
