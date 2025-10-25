using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Intergations.Implementations
{
    public class PayOsService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public PayOsService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest dto)
        {
            var payload = new
            {
                orderCode = dto.OrderCode,
                amount = dto.Amount,
                description = dto.Description,
                returnUrl = _config["PayOS:ReturnUrl"],
                cancelUrl = _config["PayOS:CancelUrl"],
                callbackUrl = _config["PayOS:CallbackUrl"]
            };

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("x-client-id", _config["PayOS:ClientId"]);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", _config["PayOS:ApiKey"]);

            var response = await _httpClient.PostAsJsonAsync(_config["PayOS:BaseUrl"], payload);

            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine("🔍 PayOS response: " + raw);

            var json = JsonDocument.Parse(raw).RootElement;

            // ✅ Check code and data safely
            var code = json.TryGetProperty("code", out var codeEl) ? codeEl.GetString() : "unknown";
            if (!json.TryGetProperty("data", out var dataEl) || dataEl.ValueKind == JsonValueKind.Null)
            {
                throw new InvalidOperationException($"PayOS response invalid. Code={code}, Body={raw}");
            }

            var checkoutUrl = dataEl.TryGetProperty("checkoutUrl", out var urlEl)
                ? urlEl.GetString()
                : null;

            if (string.IsNullOrEmpty(checkoutUrl))
                throw new InvalidOperationException($"Missing checkoutUrl in PayOS response: {raw}");

            return new PaymentResponse
            {
                CheckoutUrl = checkoutUrl,
                OrderCode = dto.OrderCode.ToString(),
                Status = "PENDING"
            };
        }
    }
}