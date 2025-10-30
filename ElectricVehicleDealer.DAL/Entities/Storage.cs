using System;
using System.Collections.Generic;

namespace ElectricVehicleDealer.DAL.Entities;

public partial class Storage
{
    public int StorageId { get; set; }
    public int? BrandId { get; set; }

    public int? VehicleId { get; set; }

    public int? StoreId { get; set; }

    public int? QuantityAvailable { get; set; }

    public DateTime? LastUpdated { get; set; }

    public virtual Store? Store { get; set; }

    public virtual Vehicle? Vehicle { get; set; }
}
