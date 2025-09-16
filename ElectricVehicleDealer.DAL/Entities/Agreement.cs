using System;
using System.Collections.Generic;

namespace ElectricVehicleDealer.DAL.Entities;

public partial class Agreement
{
    public int AgreementId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? AgreementDate { get; set; }

    public string? TermsAndConditions { get; set; }

    public string? Status { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
