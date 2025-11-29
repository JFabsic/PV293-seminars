namespace Yestino.Common.Domain;

public abstract record DomainEvent
{
    public DomainEvent(Guid aggregateId)
    {
        AggregateId = aggregateId;
    }

    public Guid AggregateId { get; set; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.Now;
}