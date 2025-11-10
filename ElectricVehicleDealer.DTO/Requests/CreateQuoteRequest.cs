using ElectricVehicleDealer.DAL.Enum;
using System;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateQuoteRequest
    {
        public int CustomerId { get; set; }
        public int VehicleId { get; set; }
        public int DealerId { get; set; }
        public decimal? TaxRate { get; set; }
        public DateTime? QuoteDate { get; set; }
        public QuoteEnum? Status { get; set; }
    }
}
