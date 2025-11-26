using ElectricVehicleDealer.DAL.Enum;
using System;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class QuoteResponse
    {
        public int QuoteId { get; set; }
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public int DealerId { get; set; }
        public int? PromotionId { get; set; }
        public decimal? TaxRate { get; set; }
        public DateTime? QuoteDate { get; set; }
        public QuoteEnum? Status { get; set; }
        public int Quantity { get; set; }
        public VehicleResponse? Vehicle { get; set; }

        public decimal? VehiclePrice { get; set; }
        public decimal? PriceWithTax { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? FinalPrice { get; set; }
    }
}
