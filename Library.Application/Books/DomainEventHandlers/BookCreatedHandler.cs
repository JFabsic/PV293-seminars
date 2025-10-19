using Library.Application.Repositories;
using Library.Domain.Common.CQRS;
using Library.Domain.DomainEvents;

namespace Library.Application.Books.DomainEventHandlers;

public class BookCreatedEventHandler(
    IBookRepository bookRepository,
    IAuthorRepository authorRepository
    ) : IDomainEventHandler<BookCreated>
{
    public async Task Handle(BookCreated domainEvent, CancellationToken cancellationToken)
    {
        // Update author statistics
        var author = await authorRepository.GetByIdAsync(domainEvent.AuthorId);

        if (author == null)
            return;

        // Increment total books published
        author.TotalBooksPublished++;
        author.LastPublishedDate = DateTime.UtcNow;

        // Update the author's most popular genre based on all their books
        var mostPopularGenre = await bookRepository.GetBooksByAuthorIdAsync(author.Id);

        author.MostPopularGenre = mostPopularGenre
            .GroupBy(b => b.Genre)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .FirstOrDefault() ?? string.Empty;

        authorRepository.Update(author);
    }
}