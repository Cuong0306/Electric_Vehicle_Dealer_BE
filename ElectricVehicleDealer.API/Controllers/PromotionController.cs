using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
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

        /// <summary>
        /// Lấy tất cả promotions (kể cả khi rỗng)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllPromotionsAsync();
            // Trả 200 luôn, kể cả khi danh sách trống
            return Ok(result);
        }

        /// <summary>
        /// Lấy promotion theo Id
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var promotion = await _service.GetPromotionByIdAsync(id);
            if (promotion == null)
                return NotFound(new { message = $"Promotion with id {id} not found" });

            return Ok(promotion);
        }

        /// <summary>
        /// Tạo mới promotion
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePromotionRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _service.CreatePromotionAsync(dto);

            // 201 Created + trả link truy cập
            return CreatedAtAction(nameof(GetById), new { id = created.PromotionId }, created);
        }

        /// <summary>
        /// Cập nhật promotion
        /// </summary>
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

        /// <summary>
        /// Xóa promotion theo Id
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeletePromotionAsync(id);
            if (!deleted)
                return NotFound(new { message = $"Promotion with id {id} not found" });

            return NoContent(); // 204 No Content
        }
    }
}
