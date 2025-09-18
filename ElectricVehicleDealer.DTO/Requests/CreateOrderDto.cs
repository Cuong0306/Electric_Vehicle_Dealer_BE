using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateOrderDto
    {
        public int CustomerId { get; set; }
        public int DealerId { get; set; }
        public int? Quantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public string? Status { get; set; }
    }
}
