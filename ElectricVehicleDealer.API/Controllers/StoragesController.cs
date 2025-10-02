using ElectricVehicleDealer.BLL.Services.Interfaces;
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
    }
}
