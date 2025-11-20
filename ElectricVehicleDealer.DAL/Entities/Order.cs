using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ElectricVehicleDealer.DAL.Enum;

namespace ElectricVehicleDealer.DAL.Entities
{
    [Table("orders")]
    public partial class Order
    {
        [Column("order_id")]
        public int OrderId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("dealer_id")]
        public int DealerId { get; set; }

        [Column("order_date")]
        public DateTime? OrderDate { get; set; }

        [Column("quantity")]
        public int? Quantity { get; set; }

        [Column("total_price")]
        public decimal? TotalPrice { get; set; }

        [Column("status")]
        public OrderEnum Status { get; set; }

        [Column("note")]
        public string? Note { get; set; }

        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(DealerId))]
        public virtual Dealer Dealer { get; set; } = null!;

        public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [Column("store_id")]
        public int? StoreId { get; set; }

        [ForeignKey("StoreId")]
        public virtual Store? Store { get; set; }
    }
}
