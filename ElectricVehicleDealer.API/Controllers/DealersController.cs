using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DealersController : ControllerBase
    {
        private readonly IDealerService _service;
        public DealersController(IDealerService service) => _service = service;

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [Authorize]
        [HttpGet("{{id}}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));


        [Authorize(Roles = "Dealer_manager, Admin")]
        [HttpPut("{{id}}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateDealerRequest dto) => Ok(await _service.UpdateAsync(id, dto));

        [Authorize(Roles = "Dealer_manager, Admin")]
        [HttpDelete("{{id}}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));

        [Authorize]
        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
    int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? status = null)
        {
            var result = await _service.GetPagedAsync(pageNumber, pageSize, search, sortBy, status);
            return Ok(result);
        }

    }
}
