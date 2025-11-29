using Microsoft.EntityFrameworkCore;
using Wolverine;
using Yestino.Common.Infrastructure.Persistence;
using Yestino.ProductCatalog.Entities;

namespace Yestino.ProductCatalog.Infrastructure;

public class ProductCatalogDbContext(DbContextOptions<ProductCatalogDbContext> options, IMessageBus bus)
    : DbContextBase(options, bus)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("product_catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductCatalogDbContext).Assembly);
    }
}