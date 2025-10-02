using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface ITestAppointmentService
    {
        Task<IEnumerable<TestAppointmentResponse>> GetAllAsync();
        Task<TestAppointmentResponse> GetByIdAsync(int id);
        Task<TestAppointmentResponse> CreateAsync(CreateTestAppointmentRequest dto);
        Task<TestAppointmentResponse> UpdateAsync(int id, UpdateTestAppointmentRequest dto);
        Task<bool> DeleteAsync(int id);
    }
}
