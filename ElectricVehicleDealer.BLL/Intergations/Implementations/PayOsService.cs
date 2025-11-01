    using ElectricVehicleDealer.BLL.Intergations.Interfaces;
    using ElectricVehicleDealer.DAL.Entities;
    using ElectricVehicleDealer.DAL.Enum;
    using ElectricVehicleDealer.DTO.Config;
    using ElectricVehicleDealer.DTO.Requests;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using System;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Intergations.Implementations
{
    public class PayOsService : IPayOsService
    {
        private readonly HttpClient _httpClient;
        private readonly PayOsSettings _settings;
        private readonly AppDbContext _appContext;

        public PayOsService(HttpClient httpClient, IOptions<PayOsSettings> settings, AppDbContext appContext)
        {
            _httpClient = httpClient;
            _settings = settings.Value;
            _appContext = appContext;
        }

        public async Task<(string qrUrl, string qrImage, DateTime expiresAt, string status, int paymentId)> CreatePaymentAsync(
            int customerId,
            int orderId,
            int amount,
            string description,
            string returnUrl,
            string cancelUrl)
        {
            long orderCode = orderId; // gán orderCode = orderId

            // 1. Tạo chữ ký
            string signature = CreateSignature(orderCode, amount, description, returnUrl, cancelUrl, _settings.ChecksumKey);

            // 2. Payload gửi PayOS
            var body = new
            {
                orderCode,
                amount,
                description,
                cancelUrl,
                returnUrl,
                items = new[] { new { name = $"Payment for order #{orderCode}", quantity = 1, price = amount } },
                signature
            };

            string json = JsonSerializer.Serialize(body);
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_settings.BaseUrl}/v2/payment-requests")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-client-id", _settings.ClientId);
            request.Headers.Add("x-api-key", _settings.ApiKey);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"PayOS error: HTTP {(int)response.StatusCode} - {content}");

            using var doc = JsonDocument.Parse(content);
            if (!doc.RootElement.TryGetProperty("data", out var data) || data.ValueKind != JsonValueKind.Object)
            {
                var code = doc.RootElement.TryGetProperty("code", out var codeEl) ? codeEl.GetString() : "unknown";
                var desc = doc.RootElement.TryGetProperty("desc", out var descEl) ? descEl.GetString() : "no description";
                throw new Exception($"PayOS error: code={code}, desc={desc}, body={content}");
            }

            string checkoutUrl = data.GetProperty("checkoutUrl").GetString() ?? "";
            string qrImage = data.TryGetProperty("qrCode", out var qrEl) ? qrEl.GetString() ?? "" : "";
            string status = data.TryGetProperty("status", out var stEl) ? stEl.GetString() ?? "PENDING" : "PENDING";

            // 3. Lưu Payment vào DB
            var payment = new Payment
            {
                CustomerId = customerId,
                OrderId = orderId,
                Amount = amount,
                Method = "PayOS",
                TransactionId = orderCode.ToString(),
                CheckoutUrl = checkoutUrl,
                Status = Enum.TryParse<PaymentEnum>(status, true, out var st) ? st : PaymentEnum.Pending,
                PaymentDate = null
            };

            _appContext.Payments.Add(payment);
            await _appContext.SaveChangesAsync();

            return (checkoutUrl, qrImage, DateTime.UtcNow.AddMinutes(30), status, payment.PaymentId);
        }

        private string CreateSignature(long orderCode, int amount, string description, string returnUrl, string cancelUrl, string key)
        {
            string rawData =
                $"amount={amount}" +
                $"&cancelUrl={cancelUrl}" +
                $"&description={description}" +
                $"&orderCode={orderCode}" +
                $"&returnUrl={returnUrl}";

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(rawData));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        // Xử lý callback từ PayOS
        public async Task HandleCallbackAsync(PayOsCallbackRequest payload)
        {
            if (payload == null)
                throw new ArgumentNullException(nameof(payload));

            // Lấy payment và order liên quan
            var payment = await _appContext.Payments
                .Include(p => p.Order)
                .FirstOrDefaultAsync(p => p.OrderId == payload.OrderCode);

            if (payment == null)
                throw new Exception($"Payment not found for OrderId {payload.OrderCode}");

            // Map status từ PayOS: hỗ trợ cả số (0,1,2,4) và text (PENDING, COMPLETED, FAILED, REFUNDED)
            PaymentEnum newStatus;

            if (int.TryParse(payload.Status, out int statusInt) && Enum.IsDefined(typeof(PaymentEnum), statusInt))
            {
                newStatus = (PaymentEnum)statusInt;
            }
            else
            {
                newStatus = payload.Status.ToUpper() switch
                {
                    "PENDING" => PaymentEnum.Pending,
                    "FAILED" => PaymentEnum.Failed,
                    "COMPLETED" => PaymentEnum.Completed,
                    "REFUNDED" => PaymentEnum.Refunded,
                    _ => PaymentEnum.Pending
                };
            }

            payment.Status = newStatus;
            payment.PaymentDate = DateTime.UtcNow.ToLocalTime();
            payment.TransactionId = payload.TransactionId;

            // Cập nhật trạng thái order dựa trên payment
            if (payment.Order != null)
            {
                switch (payment.Status)
                {
                    case PaymentEnum.Pending:
                        payment.Order.Status = OrderEnum.Pending;
                        break;
                    case PaymentEnum.Completed:
                        payment.Order.Status = OrderEnum.Completed;
                        break;
                    case PaymentEnum.Failed:
                    case PaymentEnum.Refunded:
                        payment.Order.Status = OrderEnum.Cancelled;
                        break;
                }
            }

            await _appContext.SaveChangesAsync();

            Console.WriteLine($"[PayOS Callback] OrderId={payload.OrderCode}, PaymentStatus={payment.Status}, OrderStatus={payment.Order?.Status}");
        }

    }
}
