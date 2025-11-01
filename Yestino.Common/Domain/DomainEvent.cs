namespace Yestino.Common.Domain;

public interface IDomainEvent
{
    public Guid AggregateId { get; set; }
}

public abstract record DomainEvent : IDomainEvent
{
    public DomainEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }

    public Guid AggregateId { get; set; } = Guid.NewGuid();
    public Guid AggregateVersion { get; set; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.Now;
}