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
