using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffsController : ControllerBase
    {
        private readonly IStaffService _service;
        public StaffsController(IStaffService service) => _service = service;

        
        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

       
        [HttpGet("{{id}}")]
        public async Task<IActionResult> GetById(int id) => Ok(await _service.GetByIdAsync(id));

        
        [HttpPut("{{id}}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStaffRequest dto) => Ok(await _service.UpdateAsync(id, dto));

       
        [HttpDelete("{{id}}")]
        public async Task<IActionResult> Delete(int id) => Ok(await _service.DeleteAsync(id));

        
        [HttpDelete("hard/{{id}}")]
        public async Task<IActionResult> HardDeleteUser(int staffId)
        {
            try
            {
                if (staffId <= 0)
                    return BadRequest("Invalid staff ID");
                var success = await _service.HardDeleteUserAsync(staffId);
                return success ? Ok("Staff deleted successfully") : NotFound("Staff not found or already deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("soft/{{id}}")]
        public async Task<IActionResult> SoftDeleteUser(int id)
        {

            try
            {
                if (id <= 0)
                    return BadRequest("Invalid staff ID");

                var success = await _service.SoftDeleteUserAsync(id);
                return success ? Ok("Staff deleted successfully") : NotFound("Staff not found or already deleted");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("activeStaff")]
        public async Task<IActionResult> GetAllActiveStaffs()
        {
            var staffs = await _service.GetAllActiveStaffAsync();
            return Ok(staffs);
        }
        [Authorize]
        [HttpGet("paged")]
        public async Task<IActionResult> GetStaffsPaged(
    int pageNumber = 1, int pageSize = 10, string? search = null, string? status = null)
        {
            var result = await _service.GetStaffsPagedAsync(pageNumber, pageSize, search, status);
            return Ok(result);
        }

    }
}
