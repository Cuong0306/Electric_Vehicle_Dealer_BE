using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffResponse>> GetAllAsync();
        Task<StaffResponse> GetByIdAsync(int id);
        //Task<StaffResponse?> CreateAsync(CreateStaffRequest dto);
        Task<StaffResponse> UpdateAsync(int id, UpdateStaffRequest dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteUserAsync(int id);
        Task<bool> HardDeleteUserAsync(int id);
        Task<List<StaffResponse>> GetAllActiveStaffAsync();
        Task<PagedResult<StaffResponse>> GetStaffsPagedAsync(
    int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null);
    }
}
