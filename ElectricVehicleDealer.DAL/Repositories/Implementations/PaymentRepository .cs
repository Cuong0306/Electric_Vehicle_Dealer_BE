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
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;
        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Payment>> GetPaymentsByStoreIdAsync(int storeId)
        {
            return await _context.Payments
                .Include(p => p.Order)
                .Where(p => p.Order != null && p.Order.StoreId == storeId)
                .OrderByDescending(p => p.PaymentDate ?? DateTime.MinValue)
                .ToListAsync();
        }
    }
}
