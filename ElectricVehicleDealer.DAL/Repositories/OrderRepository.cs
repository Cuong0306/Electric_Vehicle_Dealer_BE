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
    public class OrderRepository : GenericRepository<Order>
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<List<Order>> GetAllAsync()
        {
            return await _context.Orders.ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(c => c.OrderId == id);
        }

        public async Task<int> CreateAsync(CreateOrderDto dto)
        {
            var entity = new Order
            {
                CustomerId = dto.CustomerId,
                DealerId = dto.DealerId,
                Quantity = dto.Quantity,
                TotalPrice = dto.TotalPrice,
                OrderDate = DateTime.Now
            };

            await _context.Orders.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }


        public async Task<int> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var oder = await _context.Orders.FirstOrDefaultAsync(c => c.OrderId == id);
            if (oder == null) return false;
            _context.Orders.Remove(oder);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
