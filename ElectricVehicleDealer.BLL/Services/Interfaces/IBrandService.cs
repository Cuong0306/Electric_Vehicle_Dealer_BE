using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandResponse>> GetAllBrandsAsync();
        Task<BrandResponse> GetBrandByIdAsync(int id);
        Task<BrandResponse> CreateBrandAsync(CreateBrandRequest dto);
        Task<BrandResponse> UpdateBrandAsync(int id, UpdateBrandRequest dto);
        Task<bool> DeleteBrandAsync(int id);
    }
}
