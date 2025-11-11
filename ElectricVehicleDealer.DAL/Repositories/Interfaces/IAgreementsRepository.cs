using ElectricVehicleDealer.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IAgreementsRepository
    {
        Task<List<Agreement>> GetAll();
        Task<Agreement> GetByIdAsync(int id);
        Task<bool> CreateAsync(Agreement agreement);
        Task<bool> UpdateAsync(Agreement agreement);
        Task<bool> DeleteAsync(int id);
        IQueryable<Agreement> GetAllQuery();
        

    }
}
