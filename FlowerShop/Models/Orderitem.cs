using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Models;

[Table("orderitems")]
public partial class Orderitem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("orderid")]
    public int? Orderid { get; set; }

    [Column("flowerid")]
    public int? Flowerid { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [Column("unitprice")]
    [Precision(10, 2)]
    public decimal Unitprice { get; set; }

    [ForeignKey("Flowerid")]
    [InverseProperty("Orderitems")]
    public virtual Flower? Flower { get; set; }

    [ForeignKey("Orderid")]
    [InverseProperty("Orderitems")]
    public virtual Order? Order { get; set; }
}
