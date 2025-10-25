using Ardalis.Specification;
using Library.Domain.Aggregates;

namespace Library.Domain.Specifications;

/// <summary>
/// Specification to get books by a specific author
/// Domain-level business rule specification
/// </summary>
public sealed class BooksByAuthorSpec : Specification<Book>
{
    public BooksByAuthorSpec(Guid authorId)
    {
        Query.Where(b => b.AuthorId == authorId);
    }
}

/// <summary>
/// Specification to find a book by ISBN
/// Domain-level uniqueness constraint specification
/// </summary>
public sealed class BookByIsbnSpec : Specification<Book>, ISingleResultSpecification<Book>
{
    public BookByIsbnSpec(string isbn)
    {
        Query.Where(b => b.ISBN == isbn);
    }
}

/// <summary>
/// Specification to find books suitable for quick reading sessions
/// Complex query with calculated criteria (books with less than 2 hours reading time)
/// Use case: Recommend books for users with limited time
/// </summary>
public sealed class QuickReadBooksSpec : Specification<Book>
{
    private const int MaxPages = 200; // Approximately 2 hours of reading

    public QuickReadBooksSpec(bool onlyAvailable = true, string? genre = null)
    {
        // Books with 200 pages or less (approximately 2 hours reading time)
        Query.Where(b => b.Pages <= MaxPages);

        if (onlyAvailable)
        {
            Query.Where(b => b.IsAvailable);
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            Query.Where(b => b.Genre == genre);
        }

        // Order by pages ascending (shortest first)
        Query.OrderBy(b => b.Pages)
             .ThenBy(b => b.Title);
    }
}
