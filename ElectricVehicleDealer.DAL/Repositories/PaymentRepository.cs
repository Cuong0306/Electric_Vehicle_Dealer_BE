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
        public class PaymentRepository : GenericRepository<Order>
        {
            private readonly AppDbContext _context;
            public PaymentRepository(AppDbContext context) : base(context)
            {
            }
            public async Task<List<Payment>> GetAllAsync()
            {
                return await _context.Payments.ToListAsync();
            }

            public async Task<Payment> GetByIdAsync(int id)
            {
                return await _context.Payments.FirstOrDefaultAsync(c => c.PaymentId == id);
            }

            public async Task<int> CreateAsync(CreatePaymentDto dto)
            {
                var entity = new Payment
                {
                    CustomerId = dto.CustomerId,
                    OrderId = dto.OrderId,
                    Method = dto.Method,
                    Amount = dto.Amount,
                    Status = dto.Status,
                    PaymentDate = DateTime.Now
                };

                await _context.Payments.AddAsync(entity);
                return await _context.SaveChangesAsync();
            }


            public async Task<int> UpdateAsync(Payment payment)
            {
                _context.Payments.Update(payment);
                return await _context.SaveChangesAsync();
            }

            public async Task<bool> DeleteAsync(int id)
            {
                var payment = await _context.Payments.FirstOrDefaultAsync(c => c.PaymentId == id);
                if (payment == null) return false;
                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();
                return true;
            }
        }
    }