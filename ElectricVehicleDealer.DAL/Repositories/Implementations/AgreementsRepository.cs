using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class AgreementsRepository : IAgreementsRepository
    {
        private readonly AppDbContext _context;

        public AgreementsRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateAsync(Agreement agreement)
        {
            await _context.Agreements.AddAsync(agreement);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var agreement = await _context.Agreements.FirstOrDefaultAsync(t => t.AgreementId == id);
            if (agreement == null)
            {
                throw new Exception("Agreement not found");
            }

            _context.Agreements.Remove(agreement);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Agreement>> GetAll()
        {
            return await _context.Agreements
                .Include(a => a.Customer)
                .Include(a => a.Store)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Agreement> GetByIdAsync(int id)
        {
            var agreement = await _context.Agreements
                .Include(agm => agm.Customer)
                .Include(agm => agm.Store)
                .FirstOrDefaultAsync(t => t.AgreementId == id);

            if (agreement == null)
            {
                throw new Exception("Agreement not found");
            }

            return agreement;
        }

        public async Task<bool> UpdateAsync(Agreement agreement)
        {
            var existingAgreement = await _context.Agreements.FirstOrDefaultAsync(t => t.AgreementId == agreement.AgreementId);
            if (existingAgreement == null)
            {
                throw new Exception("Agreement not found");
            }

            existingAgreement.CustomerId = agreement.CustomerId;
            existingAgreement.AgreementDate = agreement.AgreementDate;
            existingAgreement.TermsAndConditions = agreement.TermsAndConditions;
            existingAgreement.Status = agreement.Status;

            if (!string.IsNullOrEmpty(agreement.FileUrl))
            {
                existingAgreement.FileUrl = agreement.FileUrl;
            }

            _context.Agreements.Update(existingAgreement);
            return await _context.SaveChangesAsync() > 0;
        }

        public IQueryable<Agreement> GetAllQuery()
        {
            return _context.Agreements.AsQueryable();
        }

        public async Task<Order?> GetOrderDetailsForAgreementAsync(int orderId)
        {
            var order = await _context.Set<Order>()
                .Where(o => o.OrderId == orderId)
                
                .Include(o => o.Quote)
                    .ThenInclude(q => q.Promotion)
                                                 
                .Include(o => o.Dealer)
                                       
                .Include(o => o.Vehicle)
                    .ThenInclude(v => v.Brand)
                .FirstOrDefaultAsync();

            return order;
        }
    }
}