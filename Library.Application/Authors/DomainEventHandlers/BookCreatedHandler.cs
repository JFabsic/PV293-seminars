using Library.Domain.Repositories;
using Library.Domain.Aggregates;
using Library.Domain.Common.CQRS;
using Library.Domain.DomainEvents;
using Library.Domain.Specifications;

namespace Library.Application.Authors.DomainEventHandlers;

public class BookCreatedEventHandler(
    IRepository<Book> bookRepository,
    IRepository<Author> authorRepository
    ) : IDomainEventHandler<BookCreated>
{
    public async Task Handle(BookCreated domainEvent, CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdAsync(domainEvent.AuthorId);

        if (author == null)
            return;

        author.TotalBooksPublished++;
        author.LastPublishedDate = DateTime.UtcNow;

        var booksByAuthorSpec = new BooksByAuthorSpec(author.Id);
        var authorBooks = await bookRepository.ListAsync(booksByAuthorSpec, cancellationToken);

        author.MostPopularGenre = authorBooks
            .GroupBy(b => b.Genre)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? string.Empty;
    }
}