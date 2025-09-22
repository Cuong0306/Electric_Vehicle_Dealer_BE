using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class UpdateBrandRequest
    {
        public string? BrandName { get; set; }

        public string? Country { get; set; }

        public string? Website { get; set; }

        public int? FounderYear { get; set; }
    }
}
