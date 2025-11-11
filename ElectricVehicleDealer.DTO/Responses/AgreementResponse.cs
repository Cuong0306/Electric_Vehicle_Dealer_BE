using ElectricVehicleDealer.DAL.Enum;
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
        public string? FileUrl { get; set; }
        public string? TermsAndConditions { get; set; }
        public int? StoreId { get; set; }
        public AgreementEnum? Status { get; set; }
        public string? CustomerName { get; set; }
        public string? FileUrl { get; set; }
        // --- Customer ---
        public CustomerResponse? Customer { get; set; }

        // --- Store ---
        public StoreResponse? Store { get; set; }

    }
}
