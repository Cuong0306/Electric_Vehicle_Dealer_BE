using ElectricVehicleDealer.DTO.Requests;
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
    }
}
