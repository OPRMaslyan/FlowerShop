using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Models;

[Table("reviews")]
public partial class Review
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("userid")]
    public int? Userid { get; set; }

    [Column("flowerid")]
    public int? Flowerid { get; set; }

    [Column("rating")]
    public int? Rating { get; set; }

    [Column("comment")]
    public string? Comment { get; set; }

    [Column("createdat", TypeName = "timestamp without time zone")]
    public DateTime? Createdat { get; set; }

    [ForeignKey("Flowerid")]
    [InverseProperty("Reviews")]
    public virtual Flower? Flower { get; set; }

    [ForeignKey("Userid")]
    [InverseProperty("Reviews")]
    public virtual User? User { get; set; }
}
