using ElectricVehicleDealer.BLL.Extensions;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DAL.Repositories.Implementations;
using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DAL.UnitOfWork;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Services.Interfaces.Implementations
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork; 
        private readonly IOrderRepository _repository;
        public OrderService(IOrderRepository repository, IUnitOfWork unitOfWork) 
        { 
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<int> CreateAsync(CreateOrderDto order)
        {
            return await _repository.CreateAsync(order); 
        }


        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var item = await _repository.GetByIdAsync(id);

                if (item != null)
                {
                    return await _repository.DeleteAsync(id);
                }
            }
            catch (Exception ex) { }
            return false;
        }

        public async Task<List<OrderResponse>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllAsync();
            }
            catch (Exception ex)
            {
                
                throw new Exception("Error when getting all customers", ex);
            }
        }

        public async Task<OrderResponse> GetByIdAsync(int id)
        {
            try
            {
                return await _repository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception("Error when getting customer by id", ex);
            }
        }

        public async Task<UpdateOrderResponse?> UpdateOrderAsync(int id, UpdateOrderRequest request)
        {
            var existingOrder = await _repository.GetEntityByIdAsync(id);
            if (existingOrder == null)
                return null;

            
            if (request.CustomerId != 0)
                existingOrder.CustomerId = request.CustomerId;

            if (request.DealerId != 0)
                existingOrder.DealerId = request.DealerId;

            if (request.OrderDate.HasValue)
                existingOrder.OrderDate = request.OrderDate;

            if (request.TotalPrice.HasValue)
                existingOrder.TotalPrice = request.TotalPrice;

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (int.TryParse(request.Status, out var numericStatus) &&
                    Enum.IsDefined(typeof(OrderEnum), numericStatus))
                {
                    existingOrder.Status = (OrderEnum)numericStatus;
                }
                else if (Enum.TryParse<OrderEnum>(request.Status, true, out var parsedStatus))
                {
                    existingOrder.Status = parsedStatus;
                }
            }


            if (!string.IsNullOrEmpty(request.Note))
                existingOrder.Note = request.Note;

            await _repository.UpdateAsync(existingOrder);

            return new UpdateOrderResponse
            {
                OrderId = existingOrder.OrderId,
                CustomerId = existingOrder.CustomerId,
                DealerId = existingOrder.DealerId,
                OrderDate = existingOrder.OrderDate,
                TotalPrice = existingOrder.TotalPrice,
                Status = existingOrder.Status.ToString(),
                Note = existingOrder.Note
            };
        }

        public async Task<List<OrderResponse>> GetOrdersByBrandIdAsync(int brandId)
        {
            try
            {
                return await _repository.GetOrdersByBrandIdAsync(brandId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when getting orders by brandId {brandId}", ex);
            }
        }
        public async Task<PagedResult<OrderResponse>> GetPagedAsync(
    int pageNumber, int pageSize, string? search = null, string? status = null)
        {
            var query = _repository.GetOrderQuery();

            // Filter search theo note, customer name hoặc dealer name
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(o =>
                    o.Note.Contains(search) ||
                    o.Customer.FullName.Contains(search) ||
                    o.Dealer.FullName.Contains(search));
            }

            // Filter status
            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<OrderEnum>(status, true, out var parsedStatus))
                {
                    query = query.Where(o => o.Status == parsedStatus);
                }
            }

            // Sort theo OrderDate giảm dần
            query = query.OrderByDescending(o => o.OrderDate);

            // Map & phân trang
            var pagedOrders = await query
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
                    }
                })
                .ToPagedResultAsync(pageNumber, pageSize);

            return pagedOrders;
        }

        public async Task<List<OrderResponse>> GetOrdersByStoreIdAsync(int storeId)
        {
            try
            {
                return await _repository.GetOrdersByStoreIdAsync(storeId);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error when getting orders by StoreId {storeId}", ex);
            }
        }

    }
}
