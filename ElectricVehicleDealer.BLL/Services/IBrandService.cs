using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services
{
    public interface IBrandService
    {
        Task<List<Brand>> GetAllAsync();
        Task<Brand> GetByIdAsync(int id);
        Task<int> CreateAsync(CreateBrandDto brandDto);
        Task<int> UpdateAsync(Brand brand);
        Task<bool> DeleteAsync(int id);
    }
}
