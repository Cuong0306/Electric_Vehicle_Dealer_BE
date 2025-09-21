using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class BrandResponse
    {
        public int BrandId { get; set; }

        public string? BrandName { get; set; }

        public string? Country { get; set; }

        public string? Website { get; set; }

        public int? FounderYear { get; set; }
    }
}
