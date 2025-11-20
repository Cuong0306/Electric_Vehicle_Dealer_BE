using System;
using System.ComponentModel.DataAnnotations.Schema;
using ElectricVehicleDealer.DAL.Enum;

namespace ElectricVehicleDealer.DAL.Entities
{
    [Table("agreements")]
    public partial class Agreement
    {
        [Column("agreement_id")]
        public int AgreementId { get; set; }

        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Column("agreement_date")]
        public DateTime? AgreementDate { get; set; }

        [Column("terms_and_conditions")]
        public string? TermsAndConditions { get; set; }

        [Column("status")]
        public AgreementEnum Status { get; set; } = AgreementEnum.Pending;

        [Column("file_url")]
        public string? FileUrl { get; set; }

        
        [ForeignKey(nameof(CustomerId))]
        public virtual Customer Customer { get; set; } = null!;

        [Column("store_id")]
        public int? StoreId { get; set; }

        [ForeignKey("StoreId")]
        public virtual Store? Store { get; set; }
    }
}
