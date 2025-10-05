using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// Nếu muốn dùng [Precision], cần EF Core 6+:
using Microsoft.EntityFrameworkCore;

namespace ElectricVehicleDealer.DAL.Entities;

[Table("vehicle")]
public partial class Vehicle
{
    [Key]
    [Column("vehicle_id")]
    public int VehicleId { get; set; }

    [Column("brand_id")]
    public int BrandId { get; set; }

    [Required]
    [MaxLength(100)]
    [Column("model_name")]
    public string ModelName { get; set; } = null!;

    [MaxLength(50)]
    [Column("version")]
    public string? Version { get; set; }

    [Column("year")]
    public int? Year { get; set; }

    [MaxLength(50)]
    [Column("color")]
    public string? Color { get; set; }

    [Precision(15, 2)]
    [Column("price", TypeName = "numeric(15,2)")]
    public decimal? Price { get; set; }

    [MaxLength(50)]
    [Column("battery_capacity")]
    public string? BatteryCapacity { get; set; }

    [MaxLength(50)]
    [Column("range_per_charge")]
    public string? RangePerCharge { get; set; }

    [MaxLength(50)]
    [Column("warranty_period")]
    public string? WarrantyPeriod { get; set; }

    // --- New fields ---
    [Column("seating_capacity")]
    public int? SeatingCapacity { get; set; }           // 4 seats

    [Column("transmission")]
    public string? Transmission { get; set; }           // Automatic

    [Column("airbags")]
    public int? Airbags { get; set; }                   // 1 airbag

    [Column("horsepower")]
    public int? Horsepower { get; set; }                // 43 HP

    [Column("vehicle_type")]
    public string? VehicleType { get; set; }            // Minicar

    [Column("trunk_capacity")]
    public int? TrunkCapacity { get; set; }             // 285 L

    [Column("daily_driving_limit")]
    public int? DailyDrivingLimit { get; set; }         // 300 km/day

    [Column("create_date")]
    public DateTime? CreateDate { get; set; }

    // Nav props
    [ForeignKey(nameof(BrandId))]
    public virtual Brand Brand { get; set; } = null!;

    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();
    public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();
    public virtual ICollection<Storage> Storages { get; set; } = new List<Storage>();
    public virtual ICollection<TestAppointment> TestAppointments { get; set; } = new List<TestAppointment>();
}
