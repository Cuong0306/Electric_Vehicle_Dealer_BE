using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories
{
    public class BrandRepository : GenericRepository<Brand>
    {
        private readonly AppDbContext _context;
        public BrandRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            return await _context.Brands.FirstOrDefaultAsync(c => c.BrandId == id);
        }

        public async Task<int> CreateAsync(CreateBrandDto dto)
        {
            var entity = new Brand
            {
                BrandName = dto.BrandName,
                Country = dto.Country,
                Website = dto.Website,
                FounderYear = dto.FounderYear
        
            };

            await _context.Brands.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }


        public async Task<int> UpdateAsync(Brand brand)
        {
            _context.Brands.Update(brand);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(c => c.BrandId == id);
            if (brand == null) return false;
            _context.Brands.Remove(brand);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
