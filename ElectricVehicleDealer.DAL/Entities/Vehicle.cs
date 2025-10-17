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

    [Column("image_urls")]
    public string[] ImageUrls { get; set; } = Array.Empty<string>();

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

    // --- Nội thất ---
    [MaxLength(100)]
    [Column("screen")]
    public string? Screen { get; set; }                 // màn hình (kích thước, OS, CarPlay/AA...)

    [MaxLength(100)]
    [Column("seat_material")]
    public string? SeatMaterial { get; set; }           // chất liệu ghế (da/nỉ/synthetic...)

    [MaxLength(100)]
    [Column("interior_material")]
    public string? InteriorMaterial { get; set; }       // chất liệu nội thất (nhựa mềm, ốp gỗ...)

    [MaxLength(100)]
    [Column("air_conditioning")]
    public string? AirConditioning { get; set; }        // Điều hòa (1 vùng/2 vùng, tự động...)

    [MaxLength(100)]
    [Column("speaker_system")]
    public string? SpeakerSystem { get; set; }          // Loa (số loa, thương hiệu)

    [MaxLength(100)]
    [Column("in_vehicle_cabinet")]
    public string? InVehicleCabinet { get; set; }       // Tủ trên xe (có/loại/kiểu bố trí)

    // --- Ngoại thất ---
    [Column("length_mm")]
    public int? LengthMm { get; set; }                  // dài (mm)

    [Column("width_mm")]
    public int? WidthMm { get; set; }                   // rộng (mm)

    [Column("height_mm")]
    public int? HeightMm { get; set; }                  // cao (mm)

    [MaxLength(100)]
    [Column("wheels")]
    public string? Wheels { get; set; }                 // bánh xe (mâm, kích thước lốp)

    [MaxLength(100)]
    [Column("headlights")]
    public string? Headlights { get; set; }             // đèn trước (Halogen/LED/Matrix...)

    [MaxLength(100)]
    [Column("taillights")]
    public string? Taillights { get; set; }             // đèn sau (LED, dạng đồ họa...)

    [MaxLength(100)]
    [Column("frame_chassis")]
    public string? FrameChassis { get; set; }           // khung xe (monocoque, body-on-frame...)

    [Column("door_count")]
    public int? DoorCount { get; set; }                 // số lượng cửa

    [MaxLength(100)]
    [Column("glass_windows")]
    public string? GlassWindows { get; set; }           // kính xe (cách nhiệt, tối màu...)

    [MaxLength(100)]
    [Column("mirrors")]
    public string? Mirrors { get; set; }                // gương (gập điện, sấy, cảnh báo điểm mù)

    [MaxLength(100)]
    [Column("cameras")]
    public string? Cameras { get; set; }
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
