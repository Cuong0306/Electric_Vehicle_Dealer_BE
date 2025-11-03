using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DAL.Repositories.Implementations
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<OrderResponse>> GetAllAsync()
        {
            var orders = await _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Quotes)
                        .ThenInclude(q => q.Vehicle)
                .Include(o => o.Dealer)
                .Include(o => o.Store)
                .Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    DealerId = o.DealerId,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status.ToString(),
                    Note = o.Note,

                    // --- Customer ---
                    Customer = o.Customer == null ? null : new CustomerResponse
                    {
                        CustomerId = o.Customer.CustomerId,
                        FullName = o.Customer.FullName,
                        Email = o.Customer.Email,
                        Phone = o.Customer.Phone,
                        Address = o.Customer.Address,
                        //DateOfBirth = o.Customer.DateOfBirth
                    },

                    // --- Dealer ---
                    Dealer = o.Dealer == null ? null : new DealerResponse
                    {
                        DealerId = o.Dealer.DealerId,
                        FullName = o.Dealer.FullName,
                        Role = o.Dealer.Role,
                        Phone = o.Dealer.Phone,
                        Email = o.Dealer.Email,
                        Address = o.Dealer.Address,
                        Status = o.Dealer.Status,
                        StoreId = o.Dealer.StoreId
                    },

                    // --- Store ---
                    Store = o.Store == null ? null : new StoreResponse
                    {
                        StoreId = o.Store.StoreId,
                        StoreName = o.Store.StoreName,
                        Email = o.Store.Email,
                        Address = o.Store.Address,
                        PromotionId = o.Store.PromotionId
                    },

                    // --- Quotes ---
                    Quotes = o.Customer.Quotes.Select(q => new QuoteResponse
                    {
                        QuoteId = q.QuoteId,
                        CustomerId = q.CustomerId,
                        DealerId = q.DealerId,
                        VehicleId = q.VehicleId,
                        QuoteDate = q.QuoteDate,
                        Status = q.Status,

                        Vehicle = q.Vehicle == null ? null : new VehicleResponse
                        {
                            VehicleId = q.Vehicle.VehicleId,
                            BrandId = q.Vehicle.BrandId,
                            ModelName = q.Vehicle.ModelName,
                            Version = q.Vehicle.Version,
                            Year = q.Vehicle.Year,
                            Color = q.Vehicle.Color,
                            Price = q.Vehicle.Price,
                            BatteryCapacity = q.Vehicle.BatteryCapacity,
                            RangePerCharge = q.Vehicle.RangePerCharge,
                            WarrantyPeriod = q.Vehicle.WarrantyPeriod,
                            SeatingCapacity = q.Vehicle.SeatingCapacity,
                            Transmission = q.Vehicle.Transmission,
                            Airbags = q.Vehicle.Airbags,
                            Horsepower = q.Vehicle.Horsepower,
                            VehicleType = q.Vehicle.VehicleType,
                            TrunkCapacity = q.Vehicle.TrunkCapacity,
                            DailyDrivingLimit = q.Vehicle.DailyDrivingLimit,
                            Screen = q.Vehicle.Screen,
                            SeatMaterial = q.Vehicle.SeatMaterial,
                            InteriorMaterial = q.Vehicle.InteriorMaterial,
                            AirConditioning = q.Vehicle.AirConditioning,
                            SpeakerSystem = q.Vehicle.SpeakerSystem,
                            InVehicleCabinet = q.Vehicle.InVehicleCabinet,
                            LengthMm = q.Vehicle.LengthMm,
                            WidthMm = q.Vehicle.WidthMm,
                            HeightMm = q.Vehicle.HeightMm,
                            Wheels = q.Vehicle.Wheels,
                            Headlights = q.Vehicle.Headlights,
                            Taillights = q.Vehicle.Taillights,
                            FrameChassis = q.Vehicle.FrameChassis,
                            DoorCount = q.Vehicle.DoorCount,
                            GlassWindows = q.Vehicle.GlassWindows,
                            Mirrors = q.Vehicle.Mirrors,
                            Cameras = q.Vehicle.Cameras,
                            IsAllocation = q.Vehicle.IsAllocation,
                            CreateDate = q.Vehicle.CreateDate,
                            //ImageUrls = q.Vehicle.VehicleImages
                            //    .Select(img => img.ImageUrl)
                            //    .ToList()
                        }
                    }).ToList()
                })
                .ToListAsync();

            return orders;
        }


        public async Task<OrderResponse?> GetByIdAsync(int id)
        {
            var order = await _context.Orders
                .Include(o => o.Customer)
                    .ThenInclude(c => c.Quotes)
                        .ThenInclude(q => q.Vehicle)
                .Include(o => o.Dealer)
                .Include(o => o.Store)
                .Where(o => o.OrderId == id)
                .Select(o => new OrderResponse
                {
                    OrderId = o.OrderId,
                    CustomerId = o.CustomerId,
                    DealerId = o.DealerId,
                    OrderDate = o.OrderDate,
                    TotalPrice = o.TotalPrice,
                    Status = o.Status.ToString(),
                    Note = o.Note,

                    Customer = o.Customer == null ? null : new CustomerResponse
                    {
                        CustomerId = o.Customer.CustomerId,
                        FullName = o.Customer.FullName,
                        Email = o.Customer.Email,
                        Phone = o.Customer.Phone,
                        Address = o.Customer.Address
                    },

                    Dealer = o.Dealer == null ? null : new DealerResponse
                    {
                        DealerId = o.Dealer.DealerId,
                        FullName = o.Dealer.FullName,
                        Role = o.Dealer.Role,
                        Phone = o.Dealer.Phone,
                        Email = o.Dealer.Email,
                        Address = o.Dealer.Address,
                        Status = o.Dealer.Status,
                        StoreId = o.Dealer.StoreId
                    },

                    Store = o.Store == null ? null : new StoreResponse
                    {
                        StoreId = o.Store.StoreId,
                        StoreName = o.Store.StoreName,
                        Email = o.Store.Email,
                        Address = o.Store.Address,
                        PromotionId = o.Store.PromotionId
                    },

                    Quotes = o.Customer.Quotes.Select(q => new QuoteResponse
                    {
                        QuoteId = q.QuoteId,
                        CustomerId = q.CustomerId,
                        DealerId = q.DealerId,
                        VehicleId = q.VehicleId,
                        QuoteDate = q.QuoteDate,
                        Status = q.Status,

                        Vehicle = q.Vehicle == null ? null : new VehicleResponse
                        {
                            VehicleId = q.Vehicle.VehicleId,
                            BrandId = q.Vehicle.BrandId,
                            ModelName = q.Vehicle.ModelName,
                            Version = q.Vehicle.Version,
                            Year = q.Vehicle.Year,
                            Color = q.Vehicle.Color,
                            Price = q.Vehicle.Price,
                            BatteryCapacity = q.Vehicle.BatteryCapacity,
                            RangePerCharge = q.Vehicle.RangePerCharge,
                            WarrantyPeriod = q.Vehicle.WarrantyPeriod,
                            SeatingCapacity = q.Vehicle.SeatingCapacity,
                            Transmission = q.Vehicle.Transmission,
                            Airbags = q.Vehicle.Airbags,
                            Horsepower = q.Vehicle.Horsepower,
                            VehicleType = q.Vehicle.VehicleType,
                            TrunkCapacity = q.Vehicle.TrunkCapacity,
                            DailyDrivingLimit = q.Vehicle.DailyDrivingLimit,
                            Screen = q.Vehicle.Screen,
                            SeatMaterial = q.Vehicle.SeatMaterial,
                            InteriorMaterial = q.Vehicle.InteriorMaterial,
                            AirConditioning = q.Vehicle.AirConditioning,
                            SpeakerSystem = q.Vehicle.SpeakerSystem,
                            InVehicleCabinet = q.Vehicle.InVehicleCabinet,
                            LengthMm = q.Vehicle.LengthMm,
                            WidthMm = q.Vehicle.WidthMm,
                            HeightMm = q.Vehicle.HeightMm,
                            Wheels = q.Vehicle.Wheels,
                            Headlights = q.Vehicle.Headlights,
                            Taillights = q.Vehicle.Taillights,
                            FrameChassis = q.Vehicle.FrameChassis,
                            DoorCount = q.Vehicle.DoorCount,
                            GlassWindows = q.Vehicle.GlassWindows,
                            Mirrors = q.Vehicle.Mirrors,
                            Cameras = q.Vehicle.Cameras,
                            IsAllocation = q.Vehicle.IsAllocation,
                            CreateDate = q.Vehicle.CreateDate
                        }
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            return order;
        }


        public async Task<int> CreateAsync(CreateOrderDto dto)
        {
            var entity = new Order
            {
                CustomerId = dto.CustomerId,
                DealerId = dto.DealerId,
                StoreId = dto.StoreId,
                Quantity = dto.Quantity,
                TotalPrice = dto.TotalPrice,
                Status = dto.Status ?? OrderEnum.Pending,
                Note = dto.Note,
                OrderDate = DateTime.Now
            };

            await _context.Orders.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }


        public async Task<int> UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var oder = await _context.Orders.FirstOrDefaultAsync(c => c.OrderId == id);
            if (oder == null) return false;
            _context.Orders.Remove(oder);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Order?> GetEntityByIdAsync(int id)
        {
            return await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == id);
        }
    }
}
