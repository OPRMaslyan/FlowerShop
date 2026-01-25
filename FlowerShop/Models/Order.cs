using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Models;

[Table("orders")]
public partial class Order
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("orderdate", TypeName = "timestamp without time zone")]
    public DateTime? Orderdate { get; set; }

    [Column("totalamount")]
    [Precision(10, 2)]
    public decimal Totalamount { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [InverseProperty("Order")]
    public virtual ICollection<Delivery> Deliveries { get; set; } = new List<Delivery>();

    [InverseProperty("Order")]
    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    [ForeignKey("Userid")]
    [InverseProperty("Orders")]
    public virtual User? User { get; set; }
}
