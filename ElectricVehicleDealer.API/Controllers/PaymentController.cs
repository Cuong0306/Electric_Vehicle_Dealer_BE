using ElectricVehicleDealer.BLL.Intergations.Interfaces;
using ElectricVehicleDealer.DAL.Entities;
using ElectricVehicleDealer.DAL.Enum;
using ElectricVehicleDealer.DTO.Config;
using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace ElectricVehicleDealer.API.Controllers
{
    [ApiController]
    [Route("api/v1/payment")]
    public class PaymentController : ControllerBase
    {
        private readonly IPayOsService _payOsService;
        private readonly PayOsSettings _payOsSettings;
        
        public PaymentController(IPayOsService payOsService, IOptions<PayOsSettings> payosOptions)
        {
            _payOsService = payOsService;
            _payOsSettings = payosOptions.Value;
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentRequest request)
        {
            var (qrUrl, qrImage, expiresAt, status, paymentId) =
                await _payOsService.CreatePaymentAsync(
                    request.CustomerId,
                    request.OrderId,
                    (int)request.Amount,
                    request.Description,
                    request.ReturnUrl,
                    request.CancelUrl
                );

            var response = new PaymentResponse
            {
                PaymentId = paymentId,
                CheckoutUrl = qrUrl,
                Amount = request.Amount,
                Status = status
            };

            return Ok(response);
        }

      
        [HttpPost("callback")]
        public async Task<IActionResult> Callback([FromBody] PayOsCallbackRequest payload)
        {
            try
            {
                await _payOsService.HandleCallbackAsync(payload);
                return Ok(new { Message = "Callback processed successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine("[PayOS Callback Error]");
                Console.WriteLine(ex);

                return StatusCode(500, new
                {
                    Message = "Failed to process callback",
                    Exception = ex.Message,
                    InnerException = ex.InnerException?.Message
                });
            }
        }


        [HttpGet("page")]
        public async Task<IActionResult> GetPayments([FromQuery] PaymentQueryRequest request)
        {
            var pagedResult = await _payOsService.GetPaymentsAsync(request);
            return Ok(pagedResult);
        }
        [HttpGet("store/{storeId}")]
        public async Task<IActionResult> GetPaymentsByStoreId(int storeId)
        {
            var payments = await _payOsService.GetPaymentsByStoreIdAsync(storeId);
            return Ok(payments);
        }

    }
}

