using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.UnitOfWork;
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
        public StaffService(IUnitOfWork uow) => _unitOfWork = uow;

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

        public async Task<StaffResponse> CreateAsync(CreateStaffRequest dto)
        {
            var entity = new Staff()
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Status = dto.Status,
                Password = dto.Password,
                Role = dto.Role ?? RoleStaffEnum.EVM_Staff
            };
            await _unitOfWork.Repository<Staff>().AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<StaffResponse> UpdateAsync(int id, UpdateStaffRequest dto)
        {
            var entity = await _unitOfWork.Repository<Staff>().GetByIdAsync(id);
            entity.FullName = dto.FullName;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Status = dto.Status;
            entity.Password = dto.Password;
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
            FullName = x.FullName,
            Phone = x.Phone,
            Email = x.Email,
            Status = x.Status,
            Password = x.Password,
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
                Password = u.Password,
                Role = u.Role

            }).ToList();
        }
    }
}
