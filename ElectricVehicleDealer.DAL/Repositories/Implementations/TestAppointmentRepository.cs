using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class TestAppointmentRepository : ITestAppointmentRepository
    {
        private readonly AppDbContext _context;
        public TestAppointmentRepository(AppDbContext context)
        {
            _context = context;
        }
        public Task AddAsync(TestAppointment entity)
        {
            throw new NotImplementedException();
        }

        public Task AddRangeAsync(IEnumerable<TestAppointment> entities)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AnyAsync(Expression<Func<TestAppointment, bool>> predicate, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestAppointment>> FindAsync(Expression<Func<TestAppointment, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<TestAppointment>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public IQueryable<TestAppointment> GetAllQuery()
        {
            throw new NotImplementedException();
        }

        public async Task<List<GetAllTestAppointmentByStoreResponse>> GetAppointmentsByStoreIdAsync(int storeId)
        {
            var appointments = await _context.TestAppointments
            .Include(t => t.Dealer) // Include Dealer info
            .Where(t => t.Dealer.StoreId == storeId)
            .Select(t => new GetAllTestAppointmentByStoreResponse
            {
                TestAppointmentId = t.TestAppointmentId,
                AppointmentDate = t.AppointmentDate,
                Status = t.Status,
                DealerName = t.Dealer.FullName,
                CustomerId = t.CustomerId,
                VehicleId = t.VehicleId,
                DealerId = t.DealerId
            })
            .ToListAsync();

            return appointments;
        }

        public Task<TestAppointment?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Remove(TestAppointment entity)
        {
            throw new NotImplementedException();
        }

        public void RemoveRange(IEnumerable<TestAppointment> entities)
        {
            throw new NotImplementedException();
        }

        public Task<TestAppointment?> SingleOrDefaultAsync(Expression<Func<TestAppointment, bool>> expression)
        {
            throw new NotImplementedException();
        }

        public void Update(TestAppointment entity)
        {
            throw new NotImplementedException();
        }
    }
}
