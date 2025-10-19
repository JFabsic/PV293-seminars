using Library.Domain.Common.CQRS;

namespace Library.Domain.DomainEvents;

public class BookCreated : IDomainEvent
{
    public Guid BookId { get; set; }
    public Guid AuthorId { get; set; }
    public string Genre { get; set; } = string.Empty;
}