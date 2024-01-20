using Microsoft.EntityFrameworkCore;
using Restore.Core.Entities;

namespace Restore.Infrastructure.Data;

public class StoreContext : DbContext
{
    public StoreContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; } = null!;
}
