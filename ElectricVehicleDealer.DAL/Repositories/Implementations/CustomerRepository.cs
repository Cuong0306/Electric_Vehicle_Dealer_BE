using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;


namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<GetAllCustomerResponse>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.Agreements)
                .Include(c => c.Orders)
                .Select(c => new GetAllCustomerResponse
                {
                    CustomerId = c.CustomerId,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    Status = c.Status,
                    Description = c.Description,
                    CreateDate = c.CreateDate,
                    Agreements = c.Agreements.Select(a => new AgreementResponse
                    {
                        AgreementId = a.AgreementId,
                        CustomerId = a.CustomerId,
                        AgreementDate = a.AgreementDate,
                        TermsAndConditions = a.TermsAndConditions,
                        StoreId = a.StoreId,
                        Status = a.Status,
                        CustomerName = c.FullName
                    }).ToList(),
                    Orders = c.Orders.Select(o => new OrderLiteResponse
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        DealerId = o.DealerId,
                        OrderDate = o.OrderDate,
                        TotalPrice = o.TotalPrice,
                        Status = o.Status.ToString(),
                        Note = o.Note
                    }).ToList()
                }).ToListAsync();
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


        public async Task<GetAllCustomerResponse?> GetByIdAsync(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.Agreements)
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

            if (customer == null) return null;

            return new GetAllCustomerResponse
            {
                    CustomerId = customer.CustomerId,
                    FullName = customer.FullName,
                    Phone = customer.Phone,
                    Email = customer.Email,
                    Address = customer.Address,
                    Status = customer.Status,
                    Description = customer.Description,
                CreateDate = customer.CreateDate,

                Agreements = customer.Agreements.Select(a => new AgreementResponse
                {
                    AgreementId = a.AgreementId,
                    CustomerId = a.CustomerId,
                    AgreementDate = a.AgreementDate,
                    TermsAndConditions = a.TermsAndConditions,
                    StoreId = a.StoreId,
                    Status = a.Status,
                    CustomerName = customer.FullName
                }).ToList(),

                Orders = customer.Orders.Select(o => new OrderLiteResponse
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    DealerId = o.DealerId,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status.ToString(),
                    Note = o.Note
                }).ToList()
            };
        }


        //public async Task<int> CreateAsync(CreateCustomerDto dto)
        //{
        //    if (dto is null) throw new ArgumentNullException(nameof(dto), "CreateCustomerDto is null");
        //    if (string.IsNullOrWhiteSpace(dto.FullName)) throw new ArgumentException("FullName is required");

        //    var entity = new Customer
        //    {
        //        FullName = dto.FullName,
        //        Phone = dto.Phone,
        //        Email = dto.Email,
        //        Address = dto.Address,
        //        LicenseUp = dto.LicenseUp,
        //        LicenseDown = dto.LicenseDown,
        //        CreateDate = DateTime.Now
        //    };

        //    // Nếu truyền storeId => gán vào bảng trung gian store_customer


        //        var store = await _context.Stores.FindAsync(dto.StoreId);
        //        if (store == null)
        //            throw new Exception($"Store with ID {dto.StoreId} not found.");

        //        entity.StoreCustomers.Add(new StoreCustomer
        //        {
        //            StoreId = dto.StoreId,
        //            Customer = entity
        //        });


        //    await _context.Customers.AddAsync(entity);
        //    return await _context.SaveChangesAsync();
        //}

        public async Task<int> CreateAsync(CreateCustomerDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto), "CreateCustomerDto is null");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                throw new ArgumentException("FullName is required");

            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new ArgumentException("Email is required to link StoreCustomer");

            var entity = new Customer
            {
                FullName = dto.FullName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                LicenseUp = dto.LicenseUp,
                LicenseDown = dto.LicenseDown,
                Status = dto.Status,
                Description = dto.Description,
                CreateDate = DateTime.Now
            };

            await _context.Customers.AddAsync(entity);
            await _context.SaveChangesAsync();

            var createdCustomer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Email == dto.Email);

            if (createdCustomer == null)
                throw new Exception($"Customer with email {dto.Email} not found after creation.");

            var store = await _context.Stores.FindAsync(dto.StoreId);
            if (store == null)
                throw new Exception($"Store with ID {dto.StoreId} not found.");

            var storeCustomer = new StoreCustomer
            {
                StoreId = dto.StoreId,
                CustomerId = createdCustomer.CustomerId
            };

            await _context.Set<StoreCustomer>().AddAsync(storeCustomer);
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

        public IQueryable<GetAllCustomerResponse> GetAllCustomerQuery()
        {
            return _context.Customers
                .Include(c => c.Agreements)
                .Include(c => c.Orders)
                .Select(c => new GetAllCustomerResponse
                {
                    CustomerId = c.CustomerId,
                    FullName = c.FullName,
                    Phone = c.Phone,
                    Email = c.Email,
                    Address = c.Address,
                    Status = c.Status,
                    Description = c.Description,
                    CreateDate = c.CreateDate,
                    Agreements = c.Agreements.Select(a => new AgreementResponse
                    {
                        AgreementId = a.AgreementId,
                        CustomerId = a.CustomerId,
                        AgreementDate = a.AgreementDate,
                        TermsAndConditions = a.TermsAndConditions,
                        StoreId = a.StoreId,
                        Status = a.Status,
                        CustomerName = c.FullName
                    }).ToList(),
                    Orders = c.Orders.Select(o => new OrderLiteResponse
                    {
                        OrderId = o.OrderId,
                        CustomerId = o.CustomerId,
                        DealerId = o.DealerId,
                        OrderDate = o.OrderDate,
                        TotalPrice = o.TotalPrice,
                        Status = o.Status.ToString(),
                        Note = o.Note
                    }).ToList()
                });
        }

    }
}
