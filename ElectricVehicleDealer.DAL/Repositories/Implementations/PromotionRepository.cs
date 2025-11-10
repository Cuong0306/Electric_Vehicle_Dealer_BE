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
    public class PromotionRepository : GenericRepository<Promotion>
    {
        private readonly AppDbContext _context;
        public PromotionRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(Promotion promotion)
        {
            await _context.Promotions.AddAsync(promotion);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var promotion = _context.Promotions.FirstOrDefault(pr => pr.PromotionId == id);
            if (promotion == null)
            {
                throw new Exception("Brand not found");
            }
            else
            {
                _context.Promotions.Remove(promotion);
                return await _context.SaveChangesAsync() > 0;
            }
        }

        public async Task<List<Promotion>> GetAll()
        {
            return await _context.Promotions
                .Include(pr => pr.Store)
                .ToListAsync();
        }

        public async Task<Promotion> GetByIdAsync(int id)
        {
            var promotion = await _context.Promotions
                .Include(pr => pr.Store)
                .FirstOrDefaultAsync(pr => pr.PromotionId == id);
            if (promotion == null)
            {
                throw new KeyNotFoundException("Promotion not found");
            }

            return promotion;
        }

        public async Task<bool> UpdateAsync(Promotion promotion)
        {
            var existingPromotion = await _context.Promotions.FirstOrDefaultAsync(pr => pr.PromotionId == promotion.PromotionId);
            if (existingPromotion == null)
            {
                throw new KeyNotFoundException("Promotion not found");
            }
            existingPromotion.Title = promotion.Title;
            existingPromotion.Description = promotion.Description;
            existingPromotion.DiscountPercent = promotion.DiscountPercent;
            existingPromotion.StartDate = promotion.StartDate;
            existingPromotion.EndDate = promotion.EndDate;


            _context.Promotions.Update(existingPromotion);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
