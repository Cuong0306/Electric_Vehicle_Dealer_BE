using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestAppointmentsController : ControllerBase
    {
        private readonly ITestAppointmentService _service;
        public TestAppointmentsController(ITestAppointmentService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpGet("{{id}}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

        [Authorize(Roles = "Dealer_staff")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTestAppointmentRequest dto) => Ok(await _service.CreateAsync(dto));

        [Authorize(Roles = "Dealer_staff")]
        [HttpPut("{{id}}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTestAppointmentRequest dto) => Ok(await _service.UpdateAsync(id, dto));

        [Authorize(Roles = "Dealer_staff")]
        [HttpDelete("{{id}}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));

        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetAppointmentsByStoreId(int storeId)
        {
            var appointments = await _service.GetAppointmentsByStoreIdAsync(storeId);
            return Ok(appointments);
        }
    }
}
