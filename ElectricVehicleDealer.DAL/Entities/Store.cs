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

    public virtual ICollection<Promotion> Promotions { get; set; } = new List<Promotion>();

    public virtual ICollection<Agreement> Agreements { get; set; } = new List<Agreement>();

    public virtual ICollection<Order> Orders{ get; set; } = new List<Order>();

    public virtual ICollection<StoreCustomer> StoreCustomers { get; set; } = new List<StoreCustomer>();
}
