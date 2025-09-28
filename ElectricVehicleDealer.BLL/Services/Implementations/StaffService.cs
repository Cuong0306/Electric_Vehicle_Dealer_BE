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
                Password = dto.Password,
                StoreId = dto.StoreId,
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
            entity.Password = dto.Password;
            entity.StoreId = dto.StoreId;
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
            Password = x.Password,
            StoreId = x.StoreId,
        };
    }
}
