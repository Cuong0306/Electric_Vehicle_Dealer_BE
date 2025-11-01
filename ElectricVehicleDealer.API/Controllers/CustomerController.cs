using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Customer>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("store/{id}/customers")]
        public async Task<ActionResult<List<Customer>>> GetCustomersByStore(int id)
        {
            var customers = await _service.GetCustomersByStoreAsync(id);

            if (customers == null || !customers.Any())
                return NotFound(new { Message = "No customers found for this store." });

            return Ok(customers);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetById(int id)
        {
            var customer = await _service.GetByIdAsync(id);
            if (customer == null) return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateCustomerDto customer)
        {
            var result = await _service.CreateAsync(customer);
            if (result > 0) return Ok("Created successfully");
            return BadRequest("Failed to create customer");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Customer customer)
        {
            if (id != customer.CustomerId) return BadRequest("ID mismatch");

            var result = await _service.UpdateAsync(customer);
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
