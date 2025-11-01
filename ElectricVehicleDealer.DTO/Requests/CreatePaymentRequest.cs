using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreatePaymentRequest
    {
        public int OrderCode { get; set; }
        public int CustomerId { get; set; }
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? ReturnUrl { get; set; }
        public string? CancelUrl { get; set; }
    }
}
