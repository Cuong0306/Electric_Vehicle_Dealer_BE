using ElectricVehicleDealer.DAL.Enum;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricVehicleDealer.DAL.Entities
{
    [Table("dealer")]
    public partial class Dealer
    {
        [Key]
        [Column("dealer_id")]
        public int DealerId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("full_name")]
        public string FullName { get; set; } = null!;

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("password")]
        public string Password { get; set; } = null!;

        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(255)]
        [Column("address")]
        public string? Address { get; set; }

        [Column("store_id")]
        public int? StoreId { get; set; }

        [Column("role")]
        public RoleDealerEnum Role { get; set; } = RoleDealerEnum.Dealer_staff;
        public string? Status { get; set; } = "Active";

        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Quote> Quotes { get; set; } = new List<Quote>();
        public virtual Store? Store { get; set; }
        public virtual ICollection<TestAppointment> TestAppointments { get; set; } = new List<TestAppointment>();
    }
}
