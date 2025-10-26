namespace Yestino.Common.Domain;

public abstract record DomainEvent
{
    public Guid AggregateId { get; set; } = Guid.NewGuid();
    public Guid AggregateVersion { get; set; } = Guid.NewGuid();
    public DateTimeOffset OccurredOn { get; } = DateTimeOffset.Now;
}