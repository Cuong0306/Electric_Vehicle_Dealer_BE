using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class PaymentQueryRequest
    {
       
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int? PaymentId { get; set; }       
        public int? CustomerId { get; set; }       
        public string? Status { get; set; }
    }
}
