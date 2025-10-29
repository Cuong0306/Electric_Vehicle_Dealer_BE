using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class LoginResponse
    {
        public required string Token { get; set; } = null!;
        public required int ExpiresIn { get; set; }
    }
}
