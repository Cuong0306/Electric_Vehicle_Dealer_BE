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

        public async Task<bool> CreateAsync(Staff staff)
        {
            await _context.Staff.AddAsync(staff);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Staff>> GetAllActiveStaffsAsync()
        {
            return await _context.Staff
                .Include(s => s.Brand)
                .Where(s => s.Status != "Deleted")
                .ToListAsync();
        }

        public async Task<Staff?> GetByEmailAsync(string email)
        {
            return await _context.Staff
                .Include(s => s.Brand)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<Staff> GetByIdAsync(int id)
        {
            return await _context.Staff.FindAsync(id);
        }

        public async Task<bool> HardDeleteUserAsync(int id)
        {
            var staff = await _context.Staff.FindAsync(id);
            if (staff == null) return false;
            _context.Staff.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsEmailExistsAsync(string email, int staffId)
        {
            return await _context.Staff.AnyAsync(s => s.Email == email && s.StaffId != staffId);
        }

        public async Task<bool> UpdateStaffAsync(Staff dto)
        {
            var staff = await _context.Staff.FindAsync(dto.StaffId);
            if (staff == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.FullName)) staff.FullName = dto.FullName;
            if (!string.IsNullOrWhiteSpace(dto.Email)) staff.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) staff.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                staff.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            if (!string.IsNullOrWhiteSpace(dto.Status)) staff.Status = dto.Status;
            //if (dto.BrandId != null) staff.BrandId = dto.BrandId;
            _context.Staff.Update(staff);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
