using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.BLL.Services.Interfaces.Implementations;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoragesController : ControllerBase
    {
        private readonly IStorageService _service;
        public StoragesController(IStorageService service) => _service = service;

        [HttpPost("allocate")]
        public async Task<IActionResult> AllocateVehicles([FromBody] AllocateVehicleDto dto)
        {
            var result = await _service.AllocateVehiclesAsync(dto);
            return Ok(new { success = result, message = "Phân bổ xe thành công." });
        }

        [HttpGet("store/{storeId}/vehicles")]
        public async Task<IActionResult> GetVehiclesByStore(int storeId)
        {
            var result = await _service.GetVehiclesByStoreIdAsync(storeId);
            return Ok(result);
        }

        [HttpPost("recall")]
        public async Task<IActionResult> RecallVehicles([FromBody] AllocateVehicleDto dto)
        {
            var result = await _service.RecallVehiclesAsync(dto);
            return Ok(new { success = result, message = "Thu hồi xe thành công." });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{{id}}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStorageRequest dto) => Ok(await _service.CreateAsync(dto));

        [HttpPut("{{id}}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStorageRequest dto) => Ok(await _service.UpdateAsync(id, dto));

        [HttpDelete("{{id}}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));
        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetStorageByBrandId(int brandId)
        {
            var result = await _service.GetStorageByBrandIdAsync(brandId);
            return Ok(result);
        }

        [HttpGet("filter")]
        public async Task<IActionResult> GetByFilter([FromQuery] int brandId = 0, [FromQuery] int storeId = 0)
        {
            var result = await _service.GetByFilterAsync(brandId, storeId);
            return Ok(result);
        }
    }
}
