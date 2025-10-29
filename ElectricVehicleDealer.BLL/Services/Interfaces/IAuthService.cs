using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest dto);
        Task<StaffResponse?> CreateStaffAsync(CreateStaffRequest dto);
        Task<DealerResponse?> CreateDealerAsync(CreateDealerRequest dto);
    }
}
