using System;
using System.Collections.Generic;

namespace ElectricVehicleDealer.DAL.Entities;

public partial class Store
{
    public int StoreId { get; set; }

    public string StoreName { get; set; } = null!;

    public string? Address { get; set; }

    public string? Email { get; set; }

    public int? PromotionId { get; set; }

    public virtual ICollection<Dealer> Dealers { get; set; } = new List<Dealer>();

    //public virtual ICollection<Staff> Staff { get; set; } = new List<Staff>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
}
