using ElectricVehicleDealer.BLL.Intergations.Implementations;
using ElectricVehicleDealer.BLL.Services.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DTO.Requests;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/v1/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly PayOsService _payOsService;

        public PaymentController(PayOsService payOsService)
        {
            _payOsService = payOsService;
        }

        // ========== TẠO GIAO DỊCH THANH TOÁN ==========
        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var result = await _payOsService.CreatePaymentAsync(request);
            return Ok(result);
        }

        // ========== NHẬN CALLBACK TỪ PAYOS ==========
        [HttpPost("callback")]
        public IActionResult Callback([FromBody] object payload)
        {
            Console.WriteLine("=== PAYOS CALLBACK ===");
            Console.WriteLine(payload);
            // TODO: verify checksum, update order status in DB
            return Ok("Callback received successfully");
        }
    }
}
