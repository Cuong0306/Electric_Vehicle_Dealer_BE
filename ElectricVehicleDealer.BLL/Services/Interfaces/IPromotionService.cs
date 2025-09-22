using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionResponse>> GetAllPromotionsAsync();
        Task<PromotionResponse> GetPromotionByIdAsync(int id);
        Task<PromotionResponse> CreatePromotionAsync(CreatePromotionRequest dto);
        Task<PromotionResponse> UpdatePromotionAsync(int id, UpdatePromotionRequest dto);
        Task<bool> DeletePromotionAsync(int id);
    }
}
