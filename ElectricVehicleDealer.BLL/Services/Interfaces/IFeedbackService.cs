using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackResponse>> GetAllAsync();
        Task<FeedbackResponse> GetByIdAsync(int id);
        Task<FeedbackResponse> CreateAsync(CreateFeedbackRequest dto);
        Task<FeedbackResponse> UpdateAsync(int id, UpdateFeedbackRequest dto);
        Task<bool> DeleteAsync(int id);
    }
}
