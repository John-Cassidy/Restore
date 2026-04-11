using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restore.Core.Entities;
using Restore.Core.Entities.OrderAggregate;

namespace Restore.Infrastructure.Data;

public class StoreContext : IdentityDbContext<User, Role, int>
{
    public StoreContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Basket> Baskets { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // use fluent entity creation to create User
        modelBuilder.Entity<User>()
            .HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey<UserAddress>(a => a.Id)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Role>()
            .HasData(
                new Role { Id = 1, Name = "Member", NormalizedName = "MEMBER", ConcurrencyStamp = "a18be9c0-aa65-4af8-bd17-00bd9344e575" },
                new Role { Id = 2, Name = "Admin", NormalizedName = "ADMIN", ConcurrencyStamp = "c7d013f0-0c8e-4cc9-b9a9-6e89a4b6a4a5" }
            );
    }
}
