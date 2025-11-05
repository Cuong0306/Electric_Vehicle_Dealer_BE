using ElectricVehicleDealer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IStorageRepository : IGenericRepository<Storage>
    {
        Task<IEnumerable<Storage>> GetStorageByFilterAsync(int? brandId, int? vehicleId);
        Task<Storage?> GetByVehicleAndStoreAsync(int vehicleId, int? storeId);
    }
}

