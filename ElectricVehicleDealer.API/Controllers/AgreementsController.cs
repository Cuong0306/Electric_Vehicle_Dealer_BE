using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AgreementsController : ControllerBase
    {
        private readonly IAgreementsService _agreementsService;
        private readonly ILogger<AgreementsController> _logger;

        public AgreementsController(
            IAgreementsService agreementsService,
            ILogger<AgreementsController> logger)
        {
            _agreementsService = agreementsService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var agreements = await _agreementsService.GetAllAgreementsAsync();
            return Ok(agreements);
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetPaged(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? search = null,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool sortDesc = false,
            [FromQuery] AgreementEnum? status = null,
            [FromQuery] int? storeId = null)
        {
            var pagedResult = await _agreementsService.GetPagedAgreementsAsync(
                pageNumber, pageSize, search, sortBy, sortDesc, status, storeId);

            return Ok(pagedResult);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var agreement = await _agreementsService.GetAgreementByIdAsync(id);
            if (agreement == null) return NotFound(new { Message = "Agreement not found" });

            return Ok(agreement);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAgreementRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var agreement = await _agreementsService.AddAgreementAsync(dto);

            return Ok(new
            {
                Message = "Agreement created successfully with PDF contract",
                Data = agreement
            });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAgreementRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _agreementsService.UpdateAgreementAsync(dto, id);
            return Ok(new { Message = "Agreement updated successfully" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _agreementsService.DeleteAgreementAsync(id);
            return Ok(new { Message = "Agreement deleted successfully" });
        }
    }
}