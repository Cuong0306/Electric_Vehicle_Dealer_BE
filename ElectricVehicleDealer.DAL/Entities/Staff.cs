using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ElectricVehicleDealer.DAL.Enum;

namespace ElectricVehicleDealer.DAL.Entities
{
    [Table("staff")]
    public partial class Staff
    {
        [Key]
        [Column("staff_id")]
        public int StaffId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("full_name")]
        public string FullName { get; set; } = null!;

        [MaxLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [MaxLength(100)]
        [Column("email")]
        public string? Email { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("password")]
        public string Password { get; set; } = null!;

        [Column("role")]
        public RoleStaffEnum Role { get; set; } = RoleStaffEnum.EVM_Staff;


        [MaxLength(100)]
        [Column("position")]
        public string? Position { get; set; }

        public string? Status { get; set; }

        [ForeignKey(nameof(Brand))]
        [Column("brand_id")]
        public int? BrandId { get; set; }

        public virtual Brand? Brand { get; set; } = null!;
    }
}
