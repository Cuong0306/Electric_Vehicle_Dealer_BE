using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Responses
{
    public class AgreementResponse
    {
        public int AgreementId { get; set; }

        public int CustomerId { get; set; }

        public DateTime? AgreementDate { get; set; }

        public string? TermsAndConditions { get; set; }

        public string? Status { get; set; }
        public string? CustomerName { get; set; }
    }
}
