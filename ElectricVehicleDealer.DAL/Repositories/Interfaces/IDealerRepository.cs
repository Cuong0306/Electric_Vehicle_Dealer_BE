using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IDealerRepository
    {
        Task<Dealer> GetByIdAsync(int id);
        Task<Dealer?> GetByEmailAsync(string email);
        Task<Boolean> IsEmailExistsAsync(string email, int dealerId);
        Task<Boolean> CreateAsync(Dealer dealer);
        Task<List<Dealer>> GetAllActiveDealersAsync();
        Task<bool> UpdateDealerAsync(Dealer dto);
        Task<bool> HardDeleteDealerAsync(int id);
        IQueryable<DealerResponse> GetAllDealerQuery();
    }
}
