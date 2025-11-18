using ElectricVehicleDealer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IVehicleRepository : IGenericRepository<Vehicle>
    {
        Task<IEnumerable<Vehicle>> GetAllByStoreIdAsync(int storeId);
        Task<IEnumerable<Vehicle>> GetAllByBrandIdAsync(int brandId);
        Task<Vehicle?> GetByIdWithIncludesAsync(int id);
    }
}
