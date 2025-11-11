using ElectricVehicleDealer.DTO.Requests;
using ElectricVehicleDealer.DTO.Responses;
using System;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.BLL.Intergations.Interfaces
{
    public interface IPayOsService
    {
        Task<(string qrUrl, string qrImage, DateTime expiresAt, string status, int paymentId)> CreatePaymentAsync(
            int customerId,
            int orderId,
            int amount,
            string description,
            string returnUrl,
            string cancelUrl
        );

        Task HandleCallbackAsync(PayOsCallbackRequest payload);

        Task<PagedResult<PaymentResponse>> GetPaymentsAsync(PaymentQueryRequest request);
        Task<List<PaymentResponse>> GetPaymentsByStoreIdAsync(int storeId);
    }
}
