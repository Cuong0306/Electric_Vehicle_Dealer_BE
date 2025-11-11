using ElectricVehicleDealer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IPaymentRepository
    {
        Task<List<Payment>> GetPaymentsByStoreIdAsync(int storeId);
    }
}
