using Azure.Core;
using ElectricVehicleDealer.BLL.Extensions;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces.Implementations
{
    public class AgreementsService : IAgreementsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly CustomerRepository _repository;
        public AgreementsService(IUnitOfWork unitOfWork, CustomerRepository repository)
        {
            _unitOfWork = unitOfWork;
            _repository = repository;
        }
        public async Task<AgreementResponse> AddAgreementAsync(CreateAgreementRequest dto)
        {
            var customer = await _repository.GetByIdAsync(dto.CustomerId);
               if(customer == null) throw new Exception("Customer" +
                   " not found");
            var newAgreement = new Agreement
            {
                CustomerId = dto.CustomerId,
                TermsAndConditions = dto.TermsAndConditions,
                Status = dto.Status ?? AgreementEnum.Pending,
                FileUrl = dto.FileUrl,
                StoreId = dto.StoreId,
                AgreementDate = dto.AgreementDate
            };
            await _unitOfWork.Agreements.CreateAsync(newAgreement);
            await _unitOfWork.SaveAsync();
            return new AgreementResponse
            {
                AgreementId = newAgreement.AgreementId,
                CustomerId = newAgreement.CustomerId,
                CustomerName = newAgreement.Customer.FullName,
                TermsAndConditions = newAgreement.TermsAndConditions,
                Status = newAgreement.Status,
                StoreId = newAgreement.StoreId,
                AgreementDate = newAgreement.AgreementDate
            };
        }

        public async Task<bool> DeleteAgreementAsync(int id)
        {
            var agreement = await _unitOfWork.Agreements.GetByIdAsync(id);
            if (agreement == null)
                throw new Exception("Agreement not found");
            var success = await _unitOfWork.Agreements.DeleteAsync(id);
            if (!success)
                throw new Exception("Failed to delete agreement");
            return true;
        }
        public async Task<AgreementResponse> GetAgreementByIdAsync(int id)
        {
            
            var agreement = await _unitOfWork.Agreements.GetByIdAsync(id);
            if(agreement == null)
            {
                throw new Exception("Agreement not found");
            }
            return new AgreementResponse 
            {
                AgreementId = agreement.AgreementId,
                CustomerId = agreement.CustomerId,
                CustomerName = agreement.Customer.FullName,
                TermsAndConditions = agreement.TermsAndConditions,
                StoreId = agreement.StoreId,
                Status = agreement.Status,
                AgreementDate = agreement.AgreementDate
            };
        }

        public async Task<AgreementResponse> GetAgreementByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AgreementResponse>> GetAllAgreementsAsync()
        {
            var agreements = await _unitOfWork.Agreements.GetAll();
            if (agreements == null || !agreements.Any())
                return new List<AgreementResponse>();
            return agreements.Select(agreement => new AgreementResponse
            {
                AgreementId = agreement.AgreementId,
                CustomerId = agreement.CustomerId,
                CustomerName = agreement.Customer.FullName,
                TermsAndConditions = agreement.TermsAndConditions,
                StoreId = agreement.StoreId,
                Status = agreement.Status,
                AgreementDate = agreement.AgreementDate
            }).ToList();
        }

        public async Task<bool> UpdateAgreementAsync(UpdateAgreementRequest dto, int id)
        {
            var agreement = await _unitOfWork.Agreements.GetByIdAsync(id);
            if (agreement == null)
                throw new Exception("Agreement not found");


            if (dto.Status.HasValue)
                agreement.Status = dto.Status.Value;

            if (!string.IsNullOrEmpty(dto.TermsAndConditions))
                agreement.TermsAndConditions = dto.TermsAndConditions;

            

            var success = await _unitOfWork.Agreements.UpdateAsync(agreement);
            if (!success)
                throw new Exception("Failed to update agreement");

            return true;
        }

        public async Task<PagedResult<AgreementResponse>> GetPagedAgreementsAsync(
    int pageNumber = 1,
    int pageSize = 10,
    string? search = null,
    string? sortBy = null,
    bool sortDesc = false,
    AgreementEnum? statusFilter = null,
    int? storeIdFilter = null)
        {
            var query = _unitOfWork.Agreements.GetAllQuery()
                .Include(a => a.Customer)
                .AsQueryable();

            // Filter theo status
            if (statusFilter.HasValue)
                query = query.Where(a => a.Status == statusFilter.Value);

            // Filter theo storeId
            if (storeIdFilter.HasValue)
                query = query.Where(a => a.StoreId == storeIdFilter.Value);

            // Search theo CustomerName hoặc TermsAndConditions
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                query = query.Where(a =>
                    a.Customer.FullName.ToLower().Contains(search) ||
                    a.TermsAndConditions.ToLower().Contains(search));
            }

            // Sort
            query = sortBy?.ToLower() switch
            {
                "status" => sortDesc ? query.OrderByDescending(a => a.Status) : query.OrderBy(a => a.Status),
                "agreementdate" => sortDesc ? query.OrderByDescending(a => a.AgreementDate) : query.OrderBy(a => a.AgreementDate),
                _ => query.OrderByDescending(a => a.AgreementDate)
            };

            var paged = await query.ToPagedResultAsync(pageNumber, pageSize);

            var mapped = paged.Items.Select(a => new AgreementResponse
            {
                AgreementId = a.AgreementId,
                CustomerId = a.CustomerId,
                CustomerName = a.Customer.FullName,
                TermsAndConditions = a.TermsAndConditions,
                StoreId = a.StoreId,
                Status = a.Status,
                AgreementDate = a.AgreementDate
            });

            return new PagedResult<AgreementResponse>
            {
                Items = mapped.ToList(),
                TotalCount = paged.TotalCount,
                PageNumber = paged.PageNumber,
                PageSize = paged.PageSize
            };
        }

    }
}
