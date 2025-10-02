using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        public FeedbackService(IUnitOfWork uow) => _unitOfWork = uow;

        public async Task<IEnumerable<FeedbackResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<Feedback>().GetAllAsync();
            return list.Select(x => MapToResponse(x));
        }

        public async Task<FeedbackResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Feedback>().GetByIdAsync(id);
            return MapToResponse(entity);
        }

        public async Task<FeedbackResponse> CreateAsync(CreateFeedbackRequest dto)
        {
            var entity = new Feedback()
            {
                CustomerId = dto.CustomerId,
                OrderId = dto.OrderId,
                VehicleId = dto.VehicleId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreateDate = dto.CreateDate,
            };
            await _unitOfWork.Repository<Feedback>().AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<FeedbackResponse> UpdateAsync(int id, UpdateFeedbackRequest dto)
        {
            var entity = await _unitOfWork.Repository<Feedback>().GetByIdAsync(id);
            entity.CustomerId = dto.CustomerId;
            entity.OrderId = dto.OrderId;
            entity.VehicleId = dto.VehicleId;
            entity.Rating = dto.Rating;
            entity.Comment = dto.Comment;
            entity.CreateDate = dto.CreateDate;
            _unitOfWork.Repository<Feedback>().Update(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Feedback>().GetByIdAsync(id);
            _unitOfWork.Repository<Feedback>().Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static FeedbackResponse MapToResponse(Feedback x) => new FeedbackResponse
        {
            FeedbackId = x.FeedbackId,
            CustomerId = x.CustomerId,
            OrderId = x.OrderId,
            VehicleId = x.VehicleId,
            Rating = x.Rating,
            Comment = x.Comment,
            CreateDate = x.CreateDate,
        };
    }
}
