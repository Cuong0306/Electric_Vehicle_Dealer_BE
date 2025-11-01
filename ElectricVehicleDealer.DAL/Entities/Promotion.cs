using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricVehicleDealer.DAL.Entities
{
    [Table("promotion")]
    public partial class Promotion
    {
        [Column("promotion_id")]
        public int PromotionId { get; set; }

        [Column("title")]
        public string? Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("discount_percent")]
        public decimal? DiscountPercent { get; set; }

        [Column("start_date")]
        public DateOnly? StartDate { get; set; }

        [Column("end_date")]
        public DateOnly? EndDate { get; set; }

        [Column("store_id")]
        public int? StoreId { get; set; } // nullable nếu promotion không bắt buộc có store

        [ForeignKey("StoreId")]
        public virtual Store? Store { get; set; }
    }
}
