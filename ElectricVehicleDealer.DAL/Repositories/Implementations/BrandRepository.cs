using ElectricVehicleDealer.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class BrandRepository : GenericRepository<Brand>
    {
        private readonly AppDbContext _context;
        public BrandRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(Brand brand)
        {
            await _context.Brands.AddAsync(brand);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var brand = _context.Brands.FirstOrDefault(b => b.BrandId == id);
            if (brand == null)
            {
                throw new Exception("Brand not found");
            }
            else
            {
                _context.Brands.Remove(brand);
                return await _context.SaveChangesAsync() > 0;
            }
        }

        public async Task<List<Brand>> GetAll()
        {
            return await _context.Brands.ToListAsync();
        }

        public async Task<Brand> GetByIdAsync(int id)
        {
            var brand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == id);
            if (brand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            return brand;
        }

        public async Task<bool> UpdateAsync(Brand brand)
        {
            var existingBrand = await _context.Brands.FirstOrDefaultAsync(b => b.BrandId == brand.BrandId);
            if (existingBrand == null)
            {
                throw new KeyNotFoundException("Brand not found");
            }

            existingBrand.BrandName = brand.BrandName;
            existingBrand.Country = brand.Country;
            existingBrand.Website = brand.Website;
            existingBrand.FounderYear = brand.FounderYear;

            _context.Brands.Update(existingBrand);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

