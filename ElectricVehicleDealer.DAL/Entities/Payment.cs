using ElectricVehicleDealer.DAL.Enum;
using System;
using System.Collections.Generic;

namespace ElectricVehicleDealer.DAL.Entities;

public partial class Payment
{
    public int PaymentId { get; set; }

    public int CustomerId { get; set; }

    public int OrderId { get; set; }

    public DateTime? PaymentDate { get; set; }

    public string? Method { get; set; }

    public decimal? Amount { get; set; }

    public PaymentEnum Status { get; set; } = PaymentEnum.Pending;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
