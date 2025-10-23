using ElectricVehicleDealer.DAL.Repositories.Interfaces;
using ElectricVehicleDealer.DAL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var token = await _authService.LoginAsync(model);
            if (token == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(new { token });
        }
    }
}
