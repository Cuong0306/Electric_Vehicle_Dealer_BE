using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces.Implementations
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        public BrandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        

        public async Task<BrandResponse> CreateBrandAsync(CreateBrandRequest dto)
        {
            var newBrand = new Brand
            {
                BrandName = dto.BrandName,
                Country = dto.Country,
                Website = dto.Website,
                FounderYear = dto.FounderYear
            };
            await _unitOfWork.Repository<Brand>().AddAsync(newBrand);
            await _unitOfWork.SaveAsync();
            return new BrandResponse
            {
                BrandId = newBrand.BrandId,
                BrandName = newBrand.BrandName,
                Country = newBrand.Country,
                Website = newBrand.Website,
                FounderYear = newBrand.FounderYear
            };
        }

        

        public async Task<bool> DeleteBrandAsync(int id)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(id);
            if (brand == null)
                throw new Exception("Brand not found");
                _unitOfWork.Repository<Brand>().Remove(brand);
                await _unitOfWork.SaveAsync();
            return true;
        }

        

        public async Task<IEnumerable<BrandResponse>> GetAllBrandsAsync()
        {
            var brands = await _unitOfWork.Repository<Brand>().GetAllAsync();
            if (brands == null || !brands.Any())
                return new List<BrandResponse>();
            return brands.Select(brands => new BrandResponse
            {
                BrandId = brands.BrandId,
                BrandName = brands.BrandName,
                Country = brands.Country,
                Website = brands.Website,
                FounderYear = brands.FounderYear
            }).ToList();
        }

        public async Task<BrandResponse> GetBrandByIdAsync(int id)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(id);
            if (brand == null)
            {
                throw new Exception("Brand not found");
            }
            return new BrandResponse
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Country = brand.Country,
                Website = brand.Website,
                FounderYear = brand.FounderYear
            };
        }

        

        public async Task<BrandResponse> UpdateBrandAsync(int id, UpdateBrandRequest dto)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetByIdAsync(id);
            if (brand == null)
                throw new Exception("Brand not found");


            if (!string.IsNullOrEmpty(dto.BrandName))
                brand.BrandName = dto.BrandName;

            if (!string.IsNullOrEmpty(dto.Country))
                brand.Country = dto.Country;
            
            if (!string.IsNullOrEmpty(dto.Website))
                brand.Website = dto.Website;

            if (dto.FounderYear.HasValue)
            {
                if (dto.FounderYear.Value <= 0)
                    throw new Exception("FounderYear must be a positive number");

                brand.FounderYear = dto.FounderYear.Value;
            }


            _unitOfWork.Repository<Brand>().Update(brand);
            await _unitOfWork.SaveAsync();

            return new BrandResponse
            {
                BrandId = brand.BrandId,
                BrandName = brand.BrandName,
                Country = brand.Country,
                Website = brand.Website,
                FounderYear = brand.FounderYear
            };
        }
    }
}
