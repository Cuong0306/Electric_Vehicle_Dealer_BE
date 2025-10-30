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
    public class VehicleRepository : GenericRepository<Vehicle>, IVehicleRepository
    {
        private readonly AppDbContext _context;
        public VehicleRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Vehicle>> GetAllByBrandIdAsync(int brandId)
        {
            return await _context.Vehicles
                                 .Where(v => v.BrandId == brandId)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetAllByStoreIdAsync(int storeId)
        {
            return await _context.Storages
                                 .Where(s => s.StoreId == storeId)
                                 .Select(s => s.Vehicle)
                                 .ToListAsync();
        }
    }
}
