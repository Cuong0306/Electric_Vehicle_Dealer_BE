using ElectricVehicleDealer.BLL.Services.Interfaces;
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

        // GET: api/agreements
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var agreements = await _agreementsService.GetAllAgreementsAsync();
                return Ok(agreements);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all agreements");
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        // GET: api/agreements/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var agreement = await _agreementsService.GetAgreementByIdAsync(id);
                if (agreement == null)
                    return NotFound(new { Message = $"Agreement with id {id} not found" });

                return Ok(agreement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting agreement by id");
                return StatusCode(500, new
                {
                    Message = "An error occurred while processing your request.",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        // POST: api/agreements
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAgreementRequest dto, int customerId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var agreement = await _agreementsService.AddAgreementAsync(dto, customerId);
                return Ok(new
                {
                    Message = "Agreement created successfully",
                    Data = agreement
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating agreement");
                return StatusCode(500, new
                {
                    Message = "Error creating Agreement",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        // DELETE: api/agreements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _agreementsService.DeleteAgreementAsync(id);
                if (!result)
                    return NotFound(new { Message = $"Agreement with id {id} not found" });

                return Ok(new { Message = $"Agreement with id {id} deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting agreement");
                return StatusCode(500, new
                {
                    Message = "Error deleting Agreement",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        // PUT: api/agreements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update([FromBody] UpdateAgreementRequest dto, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _agreementsService.UpdateAgreementAsync(dto, id);
                if (!result)
                    return NotFound(new { Message = $"Agreement with id {id} not found" });

                return Ok(new { Message = "Agreement updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating agreement");
                return StatusCode(500, new
                {
                    Message = "Error updating Agreement",
                    Error = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }


    }
}
