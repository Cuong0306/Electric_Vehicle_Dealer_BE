using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class CustomerRepository : GenericRepository<Customer>
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task<List<Customer>> GetCustomersByStoreAsync(int storeId)
        {
            if (storeId <= 0)
                throw new ArgumentException("Invalid store ID");

            var customers = await _context.StoreCustomers
                .Where(sc => sc.StoreId == storeId)
                .Select(sc => sc.Customer)
                .ToListAsync();

            return customers;
        }


        public async Task<Customer> GetByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
        }

        public async Task<int> CreateAsync(CreateCustomerDto dto)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto), "CreateCustomerDto is null");
            if (string.IsNullOrWhiteSpace(dto.FullName)) throw new ArgumentException("FullName is required");

            var entity = new Customer
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                LicenseUp = dto.LicenseUp,
                LicenseDown = dto.LicenseDown,
                CreateDate = DateTime.Now
            };

            // Nếu truyền storeId => gán vào bảng trung gian store_customer
      
            
                var store = await _context.Stores.FindAsync(dto.StoreId);
                if (store == null)
                    throw new Exception($"Store with ID {dto.StoreId} not found.");

                entity.StoreCustomers.Add(new StoreCustomer
                {
                    StoreId = dto.StoreId,
                    Customer = entity
                });
            

            await _context.Customers.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }




        public async Task<int> UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null) return false;
            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
