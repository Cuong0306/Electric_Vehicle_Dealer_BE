using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class CreateOrderResponse
    {
        public int OrderId { get; set; }
        public int PaymentId { get; set; }
        public string CheckoutUrl { get; set; }
        public string PaymentLink { get; set; }
    }
}
