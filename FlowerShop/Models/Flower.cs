using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Models;

[Table("flowers")]
public partial class Flower
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("name")]
    [StringLength(200)]
    public string Name { get; set; } = null!;

    [Column("description")]
    public string? Description { get; set; }

    [Column("price")]
    [Precision(10, 2)]
    public decimal Price { get; set; }

    [Column("stockquantity")]
    public int Stockquantity { get; set; }

    [Column("categoryid")]
    public int? Categoryid { get; set; }

    [Column("imageurl")]
    [StringLength(500)]
    public string? Imageurl { get; set; }

    [InverseProperty("Flower")]
    public virtual ICollection<Cartitem> Cartitems { get; set; } = new List<Cartitem>();

    [ForeignKey("Categoryid")]
    [InverseProperty("Flowers")]
    public virtual Category? Category { get; set; }

    [InverseProperty("Flower")]
    public virtual ICollection<Orderitem> Orderitems { get; set; } = new List<Orderitem>();

    [InverseProperty("Flower")]
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
