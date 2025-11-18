using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class QuoteRepository : GenericRepository<Quote>, IQuoteRepository
{
    public QuoteRepository(AppDbContext context) : base(context) { }

    public IQueryable<Quote> GetAllWithIncludes()
    {
        return _context.Set<Quote>()
            .Include(q => q.Customer)
            .Include(q => q.Dealer)
            .Include(q => q.Promotion)
            .Include(q => q.Vehicle)
                .ThenInclude(v => v.Brand);
    }

    public async Task<Quote?> GetByIdWithIncludesAsync(int id)
    {
        return await _context.Set<Quote>()
            .Include(q => q.Customer)
            .Include(q => q.Dealer)
            .Include(q => q.Promotion)
            .Include(q => q.Vehicle)
                .ThenInclude(v => v.Brand)
            .FirstOrDefaultAsync(q => q.QuoteId == id);
    }
}
