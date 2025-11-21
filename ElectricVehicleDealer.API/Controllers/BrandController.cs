using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElectricVehicleDealer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _service;
        public BrandsController(IBrandService service)
        {
            _service = service;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _service.GetAllBrandsAsync();
            return Ok(result);
        }

        
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var brand = await _service.GetBrandByIdAsync(id);
            return Ok(brand);
        }

  
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateBrandRequest dto)
        {
            var created = await _service.CreateBrandAsync(dto);
            return Ok(created);
        }

   
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateBrandRequest dto)
        {
            var updated = await _service.UpdateBrandAsync(id, dto);
            return Ok(updated);
        }

    
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteBrandAsync(id);
            if (!deleted) return NotFound();
            return Ok("Deleted successfully");
        }
    }
}
