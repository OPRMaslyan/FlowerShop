using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Models;

[Table("deliveries")]
public partial class Delivery
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("orderid")]
    public int? Orderid { get; set; }

    [Column("address")]
    public string Address { get; set; } = null!;

    [Column("deliverydate")]
    public DateOnly? Deliverydate { get; set; }

    [Column("status")]
    [StringLength(50)]
    public string? Status { get; set; }

    [ForeignKey("Orderid")]
    [InverseProperty("Deliveries")]
    public virtual Order? Order { get; set; }
}
