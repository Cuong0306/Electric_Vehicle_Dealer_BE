using ElectricVehicleDealer.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Interfaces
{
    public interface IQuoteRepository : IGenericRepository<Quote>   
    {
        IQueryable<Quote> GetAllWithIncludes();
        Task<Quote?> GetByIdWithIncludesAsync(int id);
    }
}
