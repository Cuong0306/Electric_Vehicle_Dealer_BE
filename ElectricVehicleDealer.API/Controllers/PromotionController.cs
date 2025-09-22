using ElectricVehicleDealer.BLL.Services.Implementations;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectricVehicleDealer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _service;
    

        public PromotionController(IPromotionService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var result = await _service.GetAllPromotionsAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var brand = await _service.GetPromotionByIdAsync(id);
            return Ok(brand);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreatePromotionRequest dto)
        {
            var created = await _service.CreatePromotionAsync(dto);
            return Ok(created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdatePromotionRequest dto)
        {
            var updated = await _service.UpdatePromotionAsync(id, dto);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _service.DeletePromotionAsync(id);
            if (!deleted) return NotFound();
            return Ok("Deleted successfully");
        }
    }
}