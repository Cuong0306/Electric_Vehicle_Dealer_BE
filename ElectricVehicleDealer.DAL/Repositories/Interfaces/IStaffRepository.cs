using ElectricVehicleDealer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        Task<Staff> GetByIdAsync(int id);
        Task<Staff?> GetByEmailAsync(string email);
        Task<Boolean> IsEmailExistsAsync(string email, int staffId);
        Task<Boolean> CreateAsync(Staff staff);
        Task<List<Staff>> GetAllActiveStaffsAsync();
        Task<bool> UpdateStaffAsync(Staff dto);
        Task<bool> HardDeleteUserAsync(int id);
        IQueryable<Staff> GetStaffQuery();
    }
}
