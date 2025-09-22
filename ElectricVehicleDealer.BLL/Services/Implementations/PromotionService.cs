using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PromotionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PromotionResponse> CreatePromotionAsync(CreatePromotionRequest dto)
        {
            var newPromotion = new Promotion
            {
                Title = dto.Title,
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };
            await _unitOfWork.Repository<Promotion>().AddAsync(newPromotion);
            await _unitOfWork.SaveAsync();
            return new PromotionResponse
            {
                PromotionId = newPromotion.PromotionId,
                Title = newPromotion.Title,
                Description = newPromotion.Description,
                DiscountPercent = newPromotion.DiscountPercent,
                StartDate = newPromotion.StartDate,
                EndDate = newPromotion.EndDate
            };
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            var promotion = await _unitOfWork.Repository<Promotion>().GetByIdAsync(id);
            if (promotion == null)
                throw new Exception("Promotion not found");
            _unitOfWork.Repository<Promotion>().Remove(promotion);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<PromotionResponse>> GetAllPromotionsAsync()
        {
            var promotions = await _unitOfWork.Repository<Promotion>().GetAllAsync();
            if (promotions == null || !promotions.Any())
                throw new Exception("No promotions found");
            return promotions.Select(p => new PromotionResponse
            {
                PromotionId = p.PromotionId,
                Title = p.Title,
                Description = p.Description,
                DiscountPercent = p.DiscountPercent,
                StartDate = p.StartDate,
                EndDate = p.EndDate
            }).ToList();
        }

        public async Task<PromotionResponse> GetPromotionByIdAsync(int id)
        {
            var promotion = await _unitOfWork.Repository<Promotion>().GetByIdAsync(id);
            if (promotion == null)
                throw new Exception("Promotion not found");
            return new PromotionResponse
            {
                PromotionId = promotion.PromotionId,
                Title = promotion.Title,
                Description = promotion.Description,
                DiscountPercent = promotion.DiscountPercent,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };
        }

        public async Task<PromotionResponse> UpdatePromotionAsync(int id, UpdatePromotionRequest dto)
        {
           var promotion = await _unitOfWork.Repository<Promotion>().GetByIdAsync(id);
            if (promotion == null)
                throw new Exception("Promotion not found");

            if (!string.IsNullOrEmpty(dto.Title))
                promotion.Title = dto.Title;
            
            if (!string.IsNullOrEmpty(dto.Description))
                promotion.Description = dto.Description;
            
            if (dto.DiscountPercent.HasValue)
                promotion.DiscountPercent = dto.DiscountPercent.Value;

            if (dto.StartDate.HasValue)
                promotion.StartDate = dto.StartDate.Value;

            if (dto.EndDate.HasValue)
                promotion.EndDate = dto.EndDate.Value;

            _unitOfWork.Repository<Promotion>().Update(promotion);
            await _unitOfWork.SaveAsync();
            return new PromotionResponse
            {
                PromotionId = promotion.PromotionId,
                Title = promotion.Title,
                Description = promotion.Description,
                DiscountPercent = promotion.DiscountPercent,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };
        }
    }
}
