using ElectricVehicleDealer.DAL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.AspNetCore.Mvc;


namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("CreateEVMStaff")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _authService.CreateStaffAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create EVM Staff failed");
                var msg = ex.Message + (ex.InnerException != null ? $" | Inner: {ex.InnerException.Message}" : "");
                return BadRequest(new { message = msg });
            }
        }

        [HttpPost("CreateDealer")]
        public async Task<IActionResult> CreateDealer([FromBody] CreateDealerRequest dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _authService.CreateDealerAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Create Dealer failed");
                var msg = ex.Message + (ex.InnerException != null ? $" | Inner: {ex.InnerException.Message}" : "");
                return BadRequest(new { message = msg });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] ElectricVehicleDealer.DTO.Requests.LoginRequest dto)
        
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return Unauthorized(new { message = "Invalid email or password" });
            }
        }
    }
}
