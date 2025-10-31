using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class StorageRepository : GenericRepository<Storage>, IStorageRepository
    {
        private readonly AppDbContext _context;
        public StorageRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Storage>> GetStorageByFilterAsync(int? brandId, int? vehicleId)
        {
            // Bắt đầu một truy vấn IQueryable
            var query = _context.Set<Storage>().AsQueryable();

            // Nếu brandId có giá trị, thêm điều kiện Where
            if (brandId.HasValue)
            {
                query = query.Where(s => s.BrandId == brandId.Value);
            }

            // Nếu storeId có giá trị, thêm điều kiện Where
            if (vehicleId.HasValue)
            {
                query = query.Where(v => v.VehicleId == vehicleId.Value);
            }

            // Thực thi truy vấn và trả về kết quả
            return await query.ToListAsync();
        }
    }
}
