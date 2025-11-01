using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Yestino.Common.Domain;

public abstract class AggregateRoot : Entity<Guid>
{
    [ConcurrencyCheck] public Guid Version { get; set; }

    protected AggregateRoot(Guid id) : base(id)
    {
    }

    protected AggregateRoot()
    {
    }

    readonly List<DomainEvent> _domainEvents = [];
    [NotMapped] public IReadOnlyList<DomainEvent> DomainEvents => _domainEvents;
    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}