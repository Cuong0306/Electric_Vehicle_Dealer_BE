using ElectricVehicleDealer.DAL.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectricVehicleDealer.DTO.Requests
{
    public class UpdateAgreementRequest
    {
        
        public string? TermsAndConditions { get; set; }
        public AgreementEnum? Status { get; set; }
    }
}
