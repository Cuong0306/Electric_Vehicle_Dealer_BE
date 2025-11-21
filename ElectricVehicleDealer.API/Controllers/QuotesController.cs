using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuotesController : ControllerBase
    {
        private readonly IQuoteService _service;
        public QuotesController(IQuoteService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{{id}}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

        [Authorize(Roles = "Dealer_staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateQuoteRequest dto) => Ok(await _service.CreateAsync(dto));

        [Authorize(Roles = "Dealer_staff")]
        [HttpPut("{{id}}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateQuoteRequest dto) => Ok(await _service.UpdateAsync(id, dto));

        [Authorize(Roles = "Dealer_staff")]
        [HttpDelete("{{id}}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
        
        [HttpGet("customer/{customerId}")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
            => Ok(await _service.GetQuotesByCustomerIdAsync(customerId));

        [HttpGet("users-with-quotes")]
        public async Task<IActionResult> GetCustomersWithQuotes()
            => Ok(await _service.GetCustomersWithQuotesAsync());
    }
}
