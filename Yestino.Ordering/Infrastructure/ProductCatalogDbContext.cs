using Microsoft.EntityFrameworkCore;
using Wolverine;
using Yestino.Common.Infrastructure.Persistence;

namespace Yestino.Ordering.Infrastructure;

public class OrderingDbContext(DbContextOptions<OrderingDbContext> options, IMessageBus bus)
    : DbContextBase(options, bus)
{

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("product_catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
    }
}