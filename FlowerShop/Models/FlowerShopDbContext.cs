using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace FlowerShop.Models;

public partial class FlowerShopDbContext : DbContext
{
    public FlowerShopDbContext()
    {
    }

    public FlowerShopDbContext(DbContextOptions<FlowerShopDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cartitem> Cartitems { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Delivery> Deliveries { get; set; }
    public virtual DbSet<Flower> Flowers { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<Orderitem> Orderitems { get; set; }
    public virtual DbSet<Review> Reviews { get; set; }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=FlowerShopDB;Username=postgres;Password=111");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cartitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("cartitems_pkey");
            entity.Property(e => e.Quantity).HasDefaultValue(1);
            entity.HasOne(d => d.Flower).WithMany(p => p.Cartitems).HasConstraintName("cartitems_flowerid_fkey");
            entity.HasOne(d => d.User).WithMany(p => p.Cartitems).HasConstraintName("cartitems_userid_fkey");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("categories_pkey");
        });

        modelBuilder.Entity<Delivery>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("deliveries_pkey");
            entity.Property(e => e.Status).HasDefaultValueSql("'Processing'::character varying");
            entity.HasOne(d => d.Order).WithMany(p => p.Deliveries).HasConstraintName("deliveries_orderid_fkey");
        });

        modelBuilder.Entity<Flower>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("flowers_pkey");
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.HasOne(d => d.Category).WithMany(p => p.Flowers).HasConstraintName("flowers_categoryid_fkey");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orders_pkey");
            entity.Property(e => e.Orderdate).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValueSql("'Pending'::character varying");
            entity.HasOne(d => d.User).WithMany(p => p.Orders).HasConstraintName("orders_userid_fkey");
        });

        modelBuilder.Entity<Orderitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("orderitems_pkey");
            entity.HasOne(d => d.Flower).WithMany(p => p.Orderitems).HasConstraintName("orderitems_flowerid_fkey");
            entity.HasOne(d => d.Order).WithMany(p => p.Orderitems).HasConstraintName("orderitems_orderid_fkey");
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("reviews_pkey");
            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(d => d.Flower).WithMany(p => p.Reviews).HasConstraintName("reviews_flowerid_fkey");
            entity.HasOne(d => d.User).WithMany(p => p.Reviews).HasConstraintName("reviews_userid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");
            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.HasOne(d => d.Role).WithMany(p => p.Users).HasConstraintName("users_roleid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}