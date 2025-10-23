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

namespace ElectricVehicleDealer.BLL.Services.Interfaces.Implementations
{
    public class StoreService : IStoreService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StoreService(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<StoreResponse> CreateStoreAsync(CreateStoreRequest dto)
        {
            var newStore = new Store
            {
                StoreName = dto.StoreName,
                Address = dto.Address,
                Email = dto.Email,
            };
            await _unitOfWork.Repository<Store>().AddAsync(newStore);
            await _unitOfWork.SaveAsync();
            return new StoreResponse
            {
                StoreId = newStore.StoreId,
                StoreName = newStore.StoreName,
                Address = newStore.Address,
                Email = newStore.Email,
                PromotionId = newStore.PromotionId,
                
            };
        }

        public async Task<bool> DeleteStoreAsync(int id)
        {
            var store = await _unitOfWork.Repository<Store>().GetByIdAsync(id);
            if (store == null)
                throw new Exception("Store not found");
            _unitOfWork.Repository<Store>().Remove(store);
            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<StoreResponse>> GetAllStoresAsync()
        {
            var stores = await _unitOfWork.Repository<Store>().GetAllAsync();
            if (stores == null || !stores.Any())
                return new List<StoreResponse>();
            return stores.Select(stores => new StoreResponse
            {
                StoreId = stores.StoreId,
                StoreName = stores.StoreName,
                Address = stores.Address,
                Email = stores.Email,
                PromotionId = stores.PromotionId
            }).ToList();
        }

        public async Task<StoreResponse> GetStoreByIdAsync(int id)
        {
            var store = await _unitOfWork.Repository<Store>().GetByIdAsync(id);
            if (store == null)
            {
                throw new Exception("Store not found");
            }
            return new StoreResponse
            {
                StoreId = store.StoreId,
                StoreName = store.StoreName,
                Address = store.Address,
                Email = store.Email,
                PromotionId = store.PromotionId
            };
        }

        public async Task<StoreResponse> UpdateStoreAsync(int id, UpdateStoreRequest dto)
        {
            var store = await _unitOfWork.Repository<Store>().GetByIdAsync(id);
            if (store == null)
                throw new Exception("Store not found");


            if (!string.IsNullOrEmpty(dto.StoreName))
                store.StoreName = dto.StoreName;

            if (!string.IsNullOrEmpty(dto.Address))
                store.Address = dto.Address;

            if (!string.IsNullOrEmpty(dto.Email))
                store.Email = dto.Email;

            if (dto.PromotionId.HasValue)
            {
                if (dto.PromotionId.Value <= 0)
                    throw new Exception("PromotionId must be a positive number");

                store.PromotionId = dto.PromotionId.Value;
            }


            _unitOfWork.Repository<Store>().Update(store);
            await _unitOfWork.SaveAsync();

            return new StoreResponse
            {
                StoreId = store.StoreId,
                StoreName = store.StoreName,
                Email = store.Email,
                Address = store.Address,
                PromotionId = store.PromotionId
            };
        }
    }
}
