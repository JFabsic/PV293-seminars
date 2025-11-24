using Microsoft.EntityFrameworkCore;
using Wolverine;
using Yestino.Common.Domain;

namespace Yestino.Common.Infrastructure.Persistence;

public abstract class DbContextBase(DbContextOptions options, IMessageBus bus) : DbContext(options)
{
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await PublishDomainEventsAndUpdateAggregateRootVersions();

        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task PublishDomainEventsAndUpdateAggregateRootVersions()
    {
        var aggregateRoots = ChangeTracker
            .Entries<AggregateRoot>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = aggregateRoots
            .SelectMany(e => e.DomainEvents)
            .ToList();

        aggregateRoots.ForEach(e => e.ClearDomainEvents());
        
        // update aggregate root version for optimistic concurrency checks
        // (we assume each aggregate publishes an event on change)
        foreach (var root in aggregateRoots)
        {
            root.Version = Guid.NewGuid();
        }

        foreach (var domainEvent in domainEvents)
        {
            await bus.PublishAsync(domainEvent);
        }
    }
}