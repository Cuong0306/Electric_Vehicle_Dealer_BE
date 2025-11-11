using ElectricVehicleDealer.BLL.Extensions;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Config;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StaffService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            
        }

        public async Task<IEnumerable<StaffResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<Staff>().GetAllAsync();
            return list.Select(x => MapToResponse(x));
        }

        public async Task<StaffResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Staff>().GetByIdAsync(id);
            return MapToResponse(entity);
        }


        public async Task<StaffResponse> UpdateAsync(int id, UpdateStaffRequest dto)
        {
            var entity = await _unitOfWork.Repository<Staff>().GetByIdAsync(id)
                 ?? throw new Exception("Staff not found");

            if (!string.IsNullOrWhiteSpace(dto.FullName)) entity.FullName = dto.FullName.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Phone)) entity.Phone = dto.Phone.Trim();
            if (!string.IsNullOrWhiteSpace(dto.Email)) entity.Email = dto.Email.Trim().ToLowerInvariant();
            if (!string.IsNullOrWhiteSpace(dto.Status)) entity.Status = dto.Status.Trim();
            if (dto.BrandId.HasValue)
                entity.BrandId = dto.BrandId.Value;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                entity.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            _unitOfWork.Repository<Staff>().Update(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Staff>().GetByIdAsync(id);
            _unitOfWork.Repository<Staff>().Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static StaffResponse MapToResponse(Staff x) => new StaffResponse
        {
            StaffId = x.StaffId,
            BrandId = x.BrandId,
            FullName = x.FullName,
            Phone = x.Phone,
            Email = x.Email,
            Status = x.Status,
            Role = x.Role
        };

        public async Task<bool> SoftDeleteUserAsync(int id)
        {
            var staff = await _unitOfWork.Repository<Staff>().GetByIdAsync(id);

            if (staff == null || staff.Status == "Deleted")
                return false;

            staff.Status = "Deleted";
            staff.Email = $"deleted_{Guid.NewGuid()}@deleted.com";
            staff.FullName = "Deleted User";
            staff.Phone = "Deleted";
            staff.Password = $"{Guid.NewGuid()}";

            await _unitOfWork.Staff.UpdateStaffAsync(staff);
            return true;
        }

        public async Task<bool> HardDeleteUserAsync(int id)
        {
            var staff = await _unitOfWork.Repository<Staff>().GetByIdAsync(id);
            if (staff != null)
            {
                var staffId = staff.StaffId;

                if (staff.Status != "Deleted")
                    throw new Exception("This Staff cannot be Hard deleted");

                var result = await _unitOfWork.Staff.HardDeleteUserAsync(id);
                if (result)
                {
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                else
                {
                    throw new Exception("Failed to delete staff");
                }
            }
            else
            {
                throw new Exception("User not found");
            }

        }

        public async Task<List<StaffResponse>> GetAllActiveStaffAsync()
        {
            var staffs = await _unitOfWork.Staff.GetAllActiveStaffsAsync();

            return staffs.Select(u => new StaffResponse
            {
                StaffId = u.StaffId,
                FullName = u.FullName,
                Phone = u.Phone,
                Email = u.Email,
                Status = u.Status,
                
                Role = u.Role

            }).ToList();
        }

        public async Task<PagedResult<StaffResponse>> GetStaffsPagedAsync(
    int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            var query = _unitOfWork.Staff.GetStaffQuery();

            // Filter theo search
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(s =>
                    s.FullName.Contains(search) ||
                    s.Email.Contains(search) ||
                    s.Phone.Contains(search));
            }

            // Filter theo status
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(s => s.Status == status);
            }

            // Sort theo FullName
            query = query.OrderBy(s => s.FullName);

            // Map & phân trang
            var pagedResult = await query
                .Select(s => new StaffResponse
                {
                    StaffId = s.StaffId,
                    FullName = s.FullName,
                    Phone = s.Phone,
                    Email = s.Email,
                    Status = s.Status,
                    Role = s.Role,
                    BrandId = s.BrandId
                })
                .ToPagedResultAsync(pageNumber, pageSize);

            return pagedResult;
        }

    }
}
