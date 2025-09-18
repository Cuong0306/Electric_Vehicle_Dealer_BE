using ElectricVehicleDealer.BLL.Services;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult> Create(CreateOrderDto order)
        {
            var result = await _service.CreateAsync(order);
            if (result > 0) return Ok("Created successfully");
            return BadRequest("Failed to create customer");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Order order)
        {
            if (id != order.CustomerId) return BadRequest("ID mismatch");

            var result = await _service.UpdateAsync(order);
            if (result > 0) return Ok("Updated successfully");
            return NotFound("Customer not found");
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
