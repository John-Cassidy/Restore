using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restore.Core.Entities;

namespace Restore.Infrastructure.Data;

public class StoreContext : IdentityDbContext<User>
{
    public StoreContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Basket> Baskets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
        .HasData(
            new IdentityRole { Name = "Member", NormalizedName = "MEMBER" },
            new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" }
        );
    }
}
