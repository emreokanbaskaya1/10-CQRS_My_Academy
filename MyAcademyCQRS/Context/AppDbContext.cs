using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyAcademyCQRS.Entities;

namespace MyAcademyCQRS.Context;

public class AppDbContext : IdentityDbContext<AppUser, AppRole, string>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Slider> Sliders { get; set; }
    public DbSet<ServiceStep> ServiceSteps { get; set; }
    public DbSet<OurHistory> OurHistories { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<PhotoGallery> PhotoGalleries { get; set; }
    public DbSet<Promotion> Promotions { get; set; }
    public DbSet<Testimonial> Testimonials { get; set; }
    public DbSet<Contact> Contacts { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<AppLog> AppLogs { get; set; }
    public DbSet<PageHeader> PageHeaders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Category → Products (1-N)
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Product
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasColumnType("decimal(18,2)");

        // Order → OrderItems (1-N)
        modelBuilder.Entity<Order>()
            .HasMany(o => o.OrderItems)
            .WithOne(oi => oi.Order)
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // OrderItem → Product (N-1)
        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrderItem>()
            .Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Order>()
            .Property(o => o.TotalPrice)
            .HasColumnType("decimal(18,2)");

        // Promotion
        modelBuilder.Entity<Promotion>()
            .Property(p => p.DiscountPercentage)
            .HasColumnType("decimal(5,2)");

        // AppLog — read-only, mapped to Serilog's auto-created table
        modelBuilder.Entity<AppLog>()
            .ToTable("AppLogs")
            .HasKey(l => l.Id);
    }
}
