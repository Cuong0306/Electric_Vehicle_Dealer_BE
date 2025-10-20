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
        Task<List<Staff>> GetAllActiveStaffsAsync();
        Task<bool> UpdateStaffAsync(Staff dto);
        Task<bool> HardDeleteUserAsync(int id);
    }
}
