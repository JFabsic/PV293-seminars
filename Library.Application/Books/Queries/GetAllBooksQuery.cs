using Ardalis.Specification;
using Library.Application.Dtos;
using Library.Domain.Repositories;
using Library.Domain.Aggregates;
using Library.Domain.Common.CQRS;

namespace Library.Application.Books.Queries;

public class GetAllBooksQuery : IQuery<List<BookDto>>;

public class GetAllBooksQueryHandler(
    IRepository<Book> bookRepository
    ) : IQueryHandler<GetAllBooksQuery, List<BookDto>>
{
    public async Task<List<BookDto>> Handle(GetAllBooksQuery query, CancellationToken cancellationToken)
    {
        // Inline specification with projection directly to DTO - efficient SQL query
        var specification = new GetAllBooksSpec();
        var books = await bookRepository.ListAsync(specification, cancellationToken);

        return [..books];
    }

    /// <summary>
    /// Query-specific specification that projects directly to BookDto
    /// Only fetches the exact fields needed from the database
    /// </summary>
    private sealed class GetAllBooksSpec : Specification<Book, BookDto>
    {
        public GetAllBooksSpec()
        {
            Query.Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                ISBN = b.ISBN,
                Year = b.Year,
                Pages = b.Pages,
                Genre = b.Genre,
                AuthorId = b.AuthorId,
                AuthorName = b.Author.Name,
                IsAvailable = b.IsAvailable,
            });
        }
    }
}