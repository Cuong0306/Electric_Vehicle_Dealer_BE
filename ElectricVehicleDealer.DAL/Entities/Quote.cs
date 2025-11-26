using ElectricVehicleDealer.DAL.Enum;
using System;
using System.Collections.Generic;

namespace ElectricVehicleDealer.DAL.Entities;

public partial class Quote
{
    public int QuoteId { get; set; }

    public int CustomerId { get; set; }

    public int VehicleId { get; set; }

    public int DealerId { get; set; }
    public decimal TaxRate { get; set; } = 0;

    public DateTime? QuoteDate { get; set; }

    public QuoteEnum Status { get; set; } = QuoteEnum.Draft;
    public int? PromotionId { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal FinalPrice { get; set; } = 0;
    public virtual Promotion? Promotion { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual Dealer Dealer { get; set; } = null!;

    public virtual Vehicle Vehicle { get; set; } = null!;
}
