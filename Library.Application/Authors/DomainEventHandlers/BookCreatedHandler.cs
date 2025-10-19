using Library.Application.Repositories;
using Library.Domain.Common.CQRS;
using Library.Domain.DomainEvents;

namespace Library.Application.Authors.DomainEventHandlers;

public class BookCreatedEventHandler(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository
    ) : IDomainEventHandler<BookCreated>
{
    public async Task Handle(BookCreated domainEvent, CancellationToken cancellationToken)
    {
        var author = await authorRepository.GetByIdAsync(domainEvent.AuthorId);

        if (author == null)
            return;

        author.TotalBooksPublished++;
        author.LastPublishedDate = DateTime.UtcNow;

        var mostPopularGenre = await bookRepository.GetBooksByAuthorIdAsync(author.Id);

        author.MostPopularGenre = mostPopularGenre
            .GroupBy(b => b.Genre)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? string.Empty;
    }
}