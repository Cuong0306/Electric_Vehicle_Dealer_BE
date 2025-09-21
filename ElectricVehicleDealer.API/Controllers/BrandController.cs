using ElectricVehicleDealer.BLL.Services;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElectricVehicleDealer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly BrandService _service;
        public BrandsController(BrandService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<Brand>>> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Brand>> GetById(int id)
        {
            var brand = await _service.GetByIdAsync(id);
            if (brand == null) return NotFound();
            return Ok(brand);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateBrandDto brand)
        {
            await _service.CreateAsync(brand);
            return Ok("Created successfully");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, Brand brand)
        {
            if (id != brand.BrandId) return BadRequest("ID mismatch");
            await _service.UpdateAsync(brand);
            return Ok("Updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return Ok("Deleted successfully");
        }
    }
}
