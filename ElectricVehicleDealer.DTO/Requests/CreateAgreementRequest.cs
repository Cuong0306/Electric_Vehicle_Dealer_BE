using ElectricVehicleDealer.DAL.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class CreateAgreementRequest
    {

        public string? TermsAndConditions { get; set; }
        public AgreementEnum? Status { get; set; }
        public int StoreId { get; set; }

        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime? AgreementDate { get; set; }

        public string? FileUrl { get; set; }

    }
}
