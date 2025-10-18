using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class StaffRepository : IStaffRepository
    {
        private readonly AppDbContext _context;
        public StaffRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<List<Staff>> GetAllActiveStaffsAsync()
        {
            return await _context.Staff
                .Where(s => s.Status != "Deleted")
                .ToListAsync();
        }

        public async Task<bool> HardDeleteUserAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return false;
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStaffAsync(Staff dto)
        {
            var staff = await _context.Staff.FindAsync(dto.StaffId);
            if (staff == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.FullName)) staff.FullName = dto.FullName;
            if (!string.IsNullOrWhiteSpace(dto.Email)) staff.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) staff.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.Password)) staff.Password = dto.Password;
            if (!string.IsNullOrWhiteSpace(dto.Status)) staff.Status = dto.Status;

            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
