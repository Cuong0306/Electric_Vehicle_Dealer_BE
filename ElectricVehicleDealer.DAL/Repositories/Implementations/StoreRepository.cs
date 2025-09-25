using ElectricVehicleDealer.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class StoreRepository : GenericRepository<Store>
    {
        private readonly AppDbContext _context;
        public StoreRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> CreateAsync(Store store)
        {
            await _context.Stores.AddAsync(store);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var store = _context.Stores.FirstOrDefault(s => s.StoreId == id);
            if (store == null)
            {
                throw new Exception("Store not found");
            }
            else
            {
                _context.Stores.Remove(store);
                return await _context.SaveChangesAsync() > 0;
            }
        }

        public async Task<List<Store>> GetAll()
        {
            return await _context.Stores.ToListAsync();
        }

        public async Task<Store> GetByIdAsync(int id)
        {
            var store = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == id);
            if (store == null)
            {
                throw new KeyNotFoundException("Store not found");
            }

            return store;
        }

        public async Task<bool> UpdateAsync(Store store)
        {
            var existingStore = await _context.Stores.FirstOrDefaultAsync(s => s.StoreId == store.StoreId);
            if (existingStore == null)
            {
                throw new KeyNotFoundException("Store not found");
            }
            existingStore.StoreName = store.StoreName;
            existingStore.Address = store.Address;
            existingStore.Email = store.Email;
            existingStore.PromotionId = store.PromotionId;

            _context.Stores.Update(existingStore);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

