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
    public class DealerService : IDealerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public DealerService(IUnitOfWork uow) => _unitOfWork = uow;

        public async Task<IEnumerable<DealerResponse>> GetAllAsync()
        {
            var list = await _unitOfWork.Repository<Dealer>().GetAllAsync();
            return list.Select(x => MapToResponse(x));
        }

        public async Task<DealerResponse> GetByIdAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Dealer>().GetByIdAsync(id);
            return MapToResponse(entity);
        }

        public async Task<DealerResponse> CreateAsync(CreateDealerRequest dto)
        {
            var entity = new Dealer()
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                StoreId = dto.StoreId,
            };
            await _unitOfWork.Repository<Dealer>().AddAsync(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<DealerResponse> UpdateAsync(int id, UpdateDealerRequest dto)
        {
            var entity = await _unitOfWork.Repository<Dealer>().GetByIdAsync(id);
            entity.FullName = dto.FullName;
            entity.Phone = dto.Phone;
            entity.Email = dto.Email;
            entity.Address = dto.Address;
            entity.StoreId = dto.StoreId;
            _unitOfWork.Repository<Dealer>().Update(entity);
            await _unitOfWork.SaveAsync();
            return MapToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Repository<Dealer>().GetByIdAsync(id);
            _unitOfWork.Repository<Dealer>().Remove(entity);
            await _unitOfWork.SaveAsync();
            return true;
        }

        private static DealerResponse MapToResponse(Dealer x) => new DealerResponse
        {
            DealerId = x.DealerId,
            FullName = x.FullName,
            Phone = x.Phone,
            Email = x.Email,
            Address = x.Address,
            StoreId = x.StoreId,
        };
    }
}
