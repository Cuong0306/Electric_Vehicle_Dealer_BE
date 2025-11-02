using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _service;
        public VehiclesController(IVehicleService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateVehicleRequest dto) => Ok(await _service.CreateAsync(dto));

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateVehicleRequest dto) => Ok(await _service.UpdateAsync(id, dto));

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetAllByStoreId(int storeId)
        {
            var vehicles = await _service.GetAllVehicleByStoreIdAsync(storeId);
            return Ok(vehicles);
        }

        // Get all vehicles by Brand ID
        [HttpGet("brand/{brandId}")]
        public async Task<IActionResult> GetAllByBrandId(int brandId)
        {
            var vehicles = await _service.GetAllVehicleByBrandIdAsync(brandId);
            return Ok(vehicles);
        }
    }
}
