using ElectricVehicleDealer.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<string?> LoginAsync(LoginRequest model);
    }
}
