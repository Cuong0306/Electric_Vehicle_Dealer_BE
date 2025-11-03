using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
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
    }
}
