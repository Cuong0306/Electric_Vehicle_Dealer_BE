using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.BLL.Services.Interfaces.Implementations;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _service;

        public OrderController(IOrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Order>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }
  
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateOrderDto dto)
        {
            try
            {
                var rows = await _service.CreateAsync(dto);
                return rows > 0 ? Ok("Created successfully") : BadRequest("No rows affected");
            }
            catch (DbUpdateException dbx)
            {
                return BadRequest(dbx.InnerException?.Message ?? dbx.Message);
            }
            catch (Exception ex)
            {
                return Problem(detail: ex.Message);
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateOrderRequest request)
        {
            var result = await _service.UpdateOrderAsync(id, request);
            if (result == null)
                return NotFound("Order not found");

            return Ok(result);
        }


        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var result = await _service.DeleteAsync(id);
            if (result) return Ok("Deleted successfully");
            return NotFound("Customer not found");
        }

        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetByBrandId(int brandId)
        {
            var result = await _service.GetOrdersByBrandIdAsync(brandId);
            if (result == null || result.Count == 0)
                return NotFound($"No orders found for brandId {brandId}");
            return Ok(result);
        }


        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetOrdersByStoreId(int storeId)
        {
            try
            {
                var orders = await _service.GetOrdersByStoreIdAsync(storeId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = $"Failed to get orders for StoreId {storeId}",
                    Exception = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
    int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search, status);
            return Ok(result);
        }

    }
}
