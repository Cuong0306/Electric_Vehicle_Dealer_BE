using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectricVehicleDealer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly IBrandService _brandService;

        public BrandController(IBrandService brandService)
        {
            _brandService = brandService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _brandService.GetAllBrandsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _brandService.GetBrandByIdAsync(id);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBrandRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _brandService.CreateBrandAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.BrandId }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateBrandRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _brandService.UpdateBrandAsync(id, request);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _brandService.DeleteBrandAsync(id);
            return success ? NoContent() : BadRequest("Failed to delete brand");
        }
    }
}
