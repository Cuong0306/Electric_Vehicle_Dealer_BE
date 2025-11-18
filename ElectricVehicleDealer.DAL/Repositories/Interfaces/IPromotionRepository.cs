using ElectricVehicleDealer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IPromotionRepository
    {
        Task<bool> CreateAsync(Promotion promotion);
        Task<Promotion> GetByIdAsync(int id);
        Task<List<Promotion>> GetAll();
        Task<bool> UpdateAsync(Promotion promotion);
        Task<bool> DeleteAsync(int id);
    }
}
