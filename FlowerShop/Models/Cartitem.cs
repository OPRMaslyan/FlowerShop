using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Models;

[Table("cartitems")]
public partial class Cartitem
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("flowerid")]
    public int? Flowerid { get; set; }

    [Column("quantity")]
    public int Quantity { get; set; }

    [ForeignKey("Flowerid")]
    [InverseProperty("Cartitems")]
    public virtual Flower? Flower { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Cartitems")]
    public virtual User? User { get; set; }
}
