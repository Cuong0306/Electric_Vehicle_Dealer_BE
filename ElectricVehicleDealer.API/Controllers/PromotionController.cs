using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

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
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPromotionsAsync();
           
            return Ok(result);
        }

        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var promotion = await _service.GetPromotionByIdAsync(id);
            if (promotion == null)
                return NotFound(new { message = $"Promotion with id {id} not found" });

            return Ok(promotion);
        }

        
   
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePromotionRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreatePromotionAsync(dto);

           
            return CreatedAtAction(nameof(GetById), new { id = created.PromotionId }, created);
        }

        
 
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePromotionRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _service.UpdatePromotionAsync(id, dto);
            if (updated == null)
                return NotFound(new { message = $"Promotion with id {id} not found" });

            return Ok(updated);
        }

        
 
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeletePromotionAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Promotion with id {id} not found" });

            return NoContent();
        }
    }
}
