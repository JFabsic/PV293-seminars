using Microsoft.EntityFrameworkCore;
using Wolverine;
using Yestino.Common.Infrastructure.Persistence;
using Yestino.Ordering.Application;
using Yestino.Ordering.Domain;

namespace Yestino.Ordering.Infrastructure;

public class OrderingDbContext(DbContextOptions<OrderingDbContext> options, IMessageBus bus)
    : DbContextBase(options, bus)
{
    public DbSet<ProductReadModel> ProductReadModels { get; set; }
    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("ordering");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderingDbContext).Assembly);
    }
}