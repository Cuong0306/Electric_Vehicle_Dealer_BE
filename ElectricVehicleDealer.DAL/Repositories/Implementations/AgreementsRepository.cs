using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
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

        public Task<bool> DeleteAsync(int id)
        {
            var agreement = _context.Agreements.FirstOrDefault(t => t.AgreementId == id);
            if (agreement == null)
            {
                throw new Exception("Agreement not found");
            }
            else
            {
                _context.Agreements.Remove(agreement);
                return Task.FromResult(_context.SaveChanges() > 0);
            }
        }

        //public async Task<List<Agreement>> GetAll()
        //{
        //    var agreements = await _context.Agreements
        //            .Include(agm => agm.Customer)
        //            .Include(agm => agm.Store)
        //            .AsNoTracking()
        //            .ToListAsync();

        //    return agreements;
        //}

        public async Task<List<AgreementResponse>> GetAll()
        {

            var agreements = await _context.Agreements
                .Include(a => a.Customer)
                .Include(a => a.Store)
                .Select(a => new AgreementResponse
                {
                    AgreementId = a.AgreementId,
                    CustomerId = a.CustomerId,
                    StoreId = a.StoreId,
                    AgreementDate = a.AgreementDate,
                    TermsAndConditions = a.TermsAndConditions,
                    Status = a.Status, // hoặc a.Status.ToString() nếu là enum

                    // --- Customer ---
                    Customer = a.Customer == null ? null : new CustomerResponse
                    {
                        CustomerId = a.Customer.CustomerId,
                        FullName = a.Customer.FullName,
                        Email = a.Customer.Email,
                        Phone = a.Customer.Phone,
                        Address = a.Customer.Address,
                        // Thêm các field khác nếu cần
                    },

                    // --- Store ---
                    Store = a.Store == null ? null : new StoreResponse
                    {
                        StoreId = a.Store.StoreId,
                        StoreName = a.Store.StoreName,
                        Email = a.Store.Email,
                        Address = a.Store.Address,
                        PromotionId = a.Store.PromotionId
                    }
                })
                .AsNoTracking()
                .ToListAsync();

            return agreements;
        }

        public Task<Agreement> GetByIdAsync(int id)
        {
            var agreement = _context.Agreements
                .Include(agm => agm.Customer)
                .FirstOrDefault(t => t.AgreementId == id);
            if (agreement == null)
            {
                throw new Exception("Agreement not found");
            }
            else
            {
                return Task.FromResult(agreement);
            }
        }

        public Task<bool> UpdateAsync(Agreement agreement)
        {
            var existingAgreement = _context.Agreements.FirstOrDefault(t => t.AgreementId == agreement.AgreementId);
            if (existingAgreement == null)
            {
                throw new Exception("Agreement not found");
            }
            else
            {
                existingAgreement.CustomerId = agreement.CustomerId;
                existingAgreement.AgreementDate = agreement.AgreementDate;
                existingAgreement.TermsAndConditions = agreement.TermsAndConditions;
                existingAgreement.Status = agreement.Status;
                _context.Agreements.Update(existingAgreement);
                return Task.FromResult(_context.SaveChanges() > 0);
            }
        }
    }
}
