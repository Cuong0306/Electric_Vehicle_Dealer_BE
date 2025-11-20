using System.ComponentModel.DataAnnotations.Schema;

namespace ElectricVehicleDealer.DAL.Entities;

[Table("store_customer")]
public class StoreCustomer
{
    [Column("store_id")]
    public int StoreId { get; set; }

    [Column("customer_id")]
    public int CustomerId { get; set; }
    public virtual Store Store { get; set; } = null!;
    public virtual Customer Customer { get; set; } = null!;
}
