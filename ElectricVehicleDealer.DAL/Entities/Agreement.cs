using ElectricVehicleDealer.DAL.Enum;
using System;
using System.Collections.Generic;

namespace ElectricVehicleDealer.DAL.Entities;

public partial class Agreement
{
    public int AgreementId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? AgreementDate { get; set; }

    public string? TermsAndConditions { get; set; }

    public AgreementEnum Status { get; set; } = AgreementEnum.Pending;

    public virtual Customer Customer { get; set; } = null!;
}
