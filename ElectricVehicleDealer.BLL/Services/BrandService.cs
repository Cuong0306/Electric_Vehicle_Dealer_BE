using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories;
using ElectricVehicleDealer.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services
{
    public class BrandService : IBrandService
    {
        private readonly BrandRepository _repository;
        public BrandService(BrandRepository repository) { _repository = repository; }
        public async Task<int> CreateAsync(CreateBrandDto brand)
        {
            try
            {
                return await _repository.CreateAsync(brand);
            }
            catch (Exception ex) { }
            return 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var item = await _repository.GetByIdAsync(id);

                if (item != null)
                {
                    return await _repository.DeleteAsync(id);
                }
            }
            catch (Exception ex) { }
            return false;
        }

        public async Task<List<Brand>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                // có thể log lỗi
                throw new Exception("Error when getting all customers", ex);
            }
        }


        public async Task<Brand> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when getting customer by id", ex);
            }
        }

        public async Task<int> UpdateAsync(Brand brand)
        {
            try
            {
                return await _repository.UpdateAsync(brand);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when updating customer", ex);
            }
        }

    }
}

