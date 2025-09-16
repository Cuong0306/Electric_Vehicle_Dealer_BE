using System;
using System.Collections.Generic;

namespace ElectricVehicleDealer.DAL.Entities;

public partial class Vehicle
{
    public int VehicleId { get; set; }

    public int BrandId { get; set; }

    public string ModelName { get; set; } = null!;

    public string? Version { get; set; }

    public int? Year { get; set; }

    public string? Color { get; set; }

    public decimal? Price { get; set; }

    public string? BatteryCapacity { get; set; }

    public string? RangePerCharge { get; set; }

    public string? WarrantyPeriod { get; set; }

    public DateTime? CreateDate { get; set; }

    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();

    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();

    public virtual ICollection<TestAppointment> TestAppointments { get; set; } = new List<TestAppointment>();
}
