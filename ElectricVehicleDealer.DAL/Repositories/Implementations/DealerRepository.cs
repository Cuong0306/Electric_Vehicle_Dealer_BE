using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class DealerRepository : IDealerRepository
    {
        private readonly AppDbContext _context;
        public DealerRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(Dealer dealer)
        {
            await _context.Dealers.AddAsync(dealer);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Dealer>> GetAllActiveDealersAsync()
        {
            return await _context.Dealers
                .Include(s => s.Store)
                .Where(s => s.Status != "Deleted")
                .ToListAsync();
        }

        public async Task<Dealer?> GetByEmailAsync(string email)
        {
            return await _context.Dealers
                .Include(d => d.Store)
                .FirstOrDefaultAsync(d => d.Email.ToLower() == email.ToLower());
        }

        public async Task<Dealer> GetByIdAsync(int id)
        {
            return await _context.Dealers.FindAsync(id);
        }

        public async Task<bool> HardDeleteDealerAsync(int id)
        {
            var dealer = await _context.Dealers.FindAsync(id);
            if (dealer == null) return false;
            _context.Dealers.Remove(dealer);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsEmailExistsAsync(string email, int dealerId)
        {
            return await _context.Dealers.AnyAsync(d => d.Email == email && d.DealerId != dealerId);
        }

        public async Task<bool> UpdateDealerAsync(Dealer dto)
        {
            var dealer = await _context.Dealers.FindAsync(dto.DealerId);
            if (dealer == null) return false;

            if (!string.IsNullOrWhiteSpace(dto.FullName)) dealer.FullName = dto.FullName;
            if (!string.IsNullOrWhiteSpace(dto.Email)) dealer.Email = dto.Email;
            if (!string.IsNullOrWhiteSpace(dto.Phone)) dealer.Phone = dto.Phone;
            if (!string.IsNullOrWhiteSpace(dto.Password))
                dealer.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            if (!string.IsNullOrWhiteSpace(dto.Status)) dealer.Status = dto.Status;
            //if (dto.StoreId.HasValue) dealer.StoreId = dto.StoreId;
            _context.Dealers.Update(dealer);
            await _context.SaveChangesAsync();
            return true;
        }

        public IQueryable<DealerResponse> GetAllDealerQuery()
        {
            return _context.Dealers
                .Include(d => d.Store)
                .Where(d => d.Status != "Deleted")
                .Select(d => new DealerResponse
                {
                    DealerId = d.DealerId,
                    FullName = d.FullName,
                    Phone = d.Phone,
                    Email = d.Email,
                    Address = d.Address,
                    StoreId = d.StoreId,
                    Status = d.Status,
                    Role = d.Role
                });
        }
    }
}
