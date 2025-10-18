using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces.Implementations
{
    public class QuoteService : IQuoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        public QuoteService(IUnitOfWork uow) => _unitOfWork = uow;

        public async Task<IEnumerable<QuoteResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<Quote>().GetAllAsync();
            return list.Select(x => MapToResponse(x));
        }

        public async Task<QuoteResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Quote>().GetByIdAsync(id);
            return MapToResponse(entity);
        }

        public async Task<QuoteResponse> CreateAsync(CreateQuoteRequest dto)
        {
            var entity = new Quote()
            {
                CustomerId = dto.CustomerId,
                VehicleId = dto.VehicleId,
                DealerId = dto.DealerId,
                QuoteDate = dto.QuoteDate,
                Status = dto.Status ?? QuoteEnum.Draft,
            };
            await _unitOfWork.Repository<Quote>().AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<QuoteResponse> UpdateAsync(int id, UpdateQuoteRequest dto)
        {
            var entity = await _unitOfWork.Repository<Quote>().GetByIdAsync(id);
            entity.CustomerId = dto.CustomerId;
            entity.VehicleId = dto.VehicleId;
            entity.DealerId = dto.DealerId;
            entity.QuoteDate = dto.QuoteDate;
            //entity.Status = dto.Status;
            _unitOfWork.Repository<Quote>().Update(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Quote>().GetByIdAsync(id);
            _unitOfWork.Repository<Quote>().Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static QuoteResponse MapToResponse(Quote x) => new QuoteResponse
        {
            QuoteId = x.QuoteId,
            CustomerId = x.CustomerId,
            VehicleId = x.VehicleId,
            DealerId = x.DealerId,
            QuoteDate = x.QuoteDate,
            Status = x.Status,
        };
    }
}
