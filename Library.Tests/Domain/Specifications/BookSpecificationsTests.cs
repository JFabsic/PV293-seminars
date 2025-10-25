using Ardalis.Specification;
using Library.Domain.Aggregates;
using Library.Domain.Specifications;

namespace Library.Tests.Domain.Specifications;

public class BookSpecificationsTests
{
    [Fact]
    public void BooksByAuthorSpec_FiltersBooksByAuthorId()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var otherAuthorId = Guid.NewGuid();

        var books = new List<Book>
        {
            Book.Create("Book 1", "ISBN-1", 2020, 200, "Fiction", authorId),
            Book.Create("Book 2", "ISBN-2", 2021, 300, "Fiction", authorId),
            Book.Create("Book 3", "ISBN-3", 2022, 250, "NonFiction", otherAuthorId),
        };

        var spec = new BooksByAuthorSpec(authorId);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, book => Assert.Equal(authorId, book.AuthorId));
        Assert.DoesNotContain(result, book => book.AuthorId == otherAuthorId);
    }

    [Fact]
    public void BooksByAuthorSpec_ReturnsEmptyWhenNoMatchingBooks()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var otherAuthorId = Guid.NewGuid();

        var books = new List<Book>
        {
            Book.Create("Book 1", "ISBN-1", 2020, 200, "Fiction", otherAuthorId),
            Book.Create("Book 2", "ISBN-2", 2021, 300, "Fiction", otherAuthorId),
        };

        var spec = new BooksByAuthorSpec(authorId);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void BooksByAuthorSpec_WorksWithEmptyGuid()
    {
        // Arrange
        var emptyGuid = Guid.Empty;
        var authorId = Guid.NewGuid();

        var books = new List<Book>
        {
            Book.Create("Book 1", "ISBN-1", 2020, 200, "Fiction", authorId),
        };

        var spec = new BooksByAuthorSpec(emptyGuid);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void BookByIsbnSpec_FindsBookByExactIsbn()
    {
        // Arrange
        var targetIsbn = "978-0-123456-78-9";

        var books = new List<Book>
        {
            Book.Create("Book 1", targetIsbn, 2020, 200, "Fiction", Guid.NewGuid()),
            Book.Create("Book 2", "978-0-987654-32-1", 2021, 300, "Fiction", Guid.NewGuid()),
        };

        var spec = new BookByIsbnSpec(targetIsbn);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        var book = Assert.Single(result);
        Assert.Equal(targetIsbn, book.ISBN);
        Assert.Equal("Book 1", book.Title);
    }

    [Fact]
    public void BookByIsbnSpec_ReturnsEmptyWhenIsbnNotFound()
    {
        // Arrange
        var targetIsbn = "978-0-123456-78-9";

        var books = new List<Book>
        {
            Book.Create("Book 1", "978-0-111111-11-1", 2020, 200, "Fiction", Guid.NewGuid()),
            Book.Create("Book 2", "978-0-222222-22-2", 2021, 300, "Fiction", Guid.NewGuid()),
        };

        var spec = new BookByIsbnSpec(targetIsbn);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void BookByIsbnSpec_IsCaseSensitive()
    {
        // Arrange
        var targetIsbn = "ISBN-123";

        var books = new List<Book>
        {
            Book.Create("Book 1", "isbn-123", 2020, 200, "Fiction", Guid.NewGuid()),
            Book.Create("Book 2", "ISBN-123", 2021, 300, "Fiction", Guid.NewGuid()),
        };

        var spec = new BookByIsbnSpec(targetIsbn);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        var book = Assert.Single(result);
        Assert.Equal("ISBN-123", book.ISBN);
        Assert.Equal("Book 2", book.Title);
    }

    [Fact]
    public void BookByIsbnSpec_IsSingleResultSpecification()
    {
        // Arrange
        var spec = new BookByIsbnSpec("978-0-123456-78-9");

        // Assert - verify it implements ISingleResultSpecification
        Assert.IsAssignableFrom<ISingleResultSpecification<Book>>(spec);
    }

    [Fact]
    public void BookByIsbnSpec_HandlesNullIsbn()
    {
        // Arrange
        var books = new List<Book>
        {
            Book.Create("Book 1", "ISBN-123", 2020, 200, "Fiction", Guid.NewGuid()),
        };

        var spec = new BookByIsbnSpec(null!);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert - should not throw and return empty
        Assert.Empty(result);
    }

    [Fact]
    public void BookByIsbnSpec_HandlesEmptyString()
    {
        // Arrange
        var books = new List<Book>
        {
            Book.Create("Book 1", "ISBN-123", 2020, 200, "Fiction", Guid.NewGuid()),
            Book.Create("Book 2", "", 2021, 300, "Fiction", Guid.NewGuid()),
        };

        var spec = new BookByIsbnSpec("");

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        var book = Assert.Single(result);
        Assert.Equal("", book.ISBN);
    }
    
    [Fact]
    public void QuickReadBooksSpec_FiltersBooksByPageCount()
    {
        // Arrange
        var books = new List<Book>
        {
            Book.Create("Quick Read 1", "ISBN-1", 2020, 100, "Fiction", Guid.NewGuid()),
            Book.Create("Quick Read 2", "ISBN-2", 2021, 200, "Fiction", Guid.NewGuid()),
            Book.Create("Long Read", "ISBN-3", 2022, 500, "Fiction", Guid.NewGuid()),
        };

        var spec = new QuickReadBooksSpec();

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, book => Assert.True(book.Pages <= 200));
        Assert.DoesNotContain(result, b => b.Title == "Long Read");
    }

    [Fact]
    public void QuickReadBooksSpec_FiltersOnlyAvailableByDefault()
    {
        // Arrange
        var availableBook = Book.Create("Available Quick", "ISBN-1", 2020, 150, "Fiction", Guid.NewGuid());
        var unavailableBook = Book.Create("Unavailable Quick", "ISBN-2", 2021, 180, "Fiction", Guid.NewGuid());
        unavailableBook.UnmarkAsLoaned();

        var books = new List<Book> { availableBook, unavailableBook };

        var spec = new QuickReadBooksSpec(onlyAvailable: true);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        var book = Assert.Single(result);
        Assert.Equal("Available Quick", book.Title);
        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void QuickReadBooksSpec_IncludesUnavailableWhenSpecified()
    {
        // Arrange
        var availableBook = Book.Create("Available Quick", "ISBN-1", 2020, 150, "Fiction", Guid.NewGuid());
        var unavailableBook = Book.Create("Unavailable Quick", "ISBN-2", 2021, 180, "Fiction", Guid.NewGuid());
        unavailableBook.UnmarkAsLoaned();

        var books = new List<Book> { availableBook, unavailableBook };

        var spec = new QuickReadBooksSpec(onlyAvailable: false);

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void QuickReadBooksSpec_FiltersByGenre()
    {
        // Arrange
        var books = new List<Book>
        {
            Book.Create("Quick Fiction", "ISBN-1", 2020, 100, "Fiction", Guid.NewGuid()),
            Book.Create("Quick Mystery", "ISBN-2", 2021, 150, "Mystery", Guid.NewGuid()),
            Book.Create("Quick SciFi", "ISBN-3", 2022, 200, "Science Fiction", Guid.NewGuid()),
        };

        var spec = new QuickReadBooksSpec(genre: "Mystery");

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        var book = Assert.Single(result);
        Assert.Equal("Quick Mystery", book.Title);
        Assert.Equal("Mystery", book.Genre);
    }

    [Fact]
    public void QuickReadBooksSpec_OrdersByPagesAscending()
    {
        // Arrange
        var books = new List<Book>
        {
            Book.Create("Medium Book", "ISBN-1", 2020, 150, "Fiction", Guid.NewGuid()),
            Book.Create("Short Book", "ISBN-2", 2021, 50, "Fiction", Guid.NewGuid()),
            Book.Create("Long-ish Book", "ISBN-3", 2022, 200, "Fiction", Guid.NewGuid()),
        };

        var spec = new QuickReadBooksSpec();

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal("Short Book", result[0].Title);
        Assert.Equal(50, result[0].Pages);
        Assert.Equal("Medium Book", result[1].Title);
        Assert.Equal(150, result[1].Pages);
        Assert.Equal("Long-ish Book", result[2].Title);
        Assert.Equal(200, result[2].Pages);
    }

    [Fact]
    public void QuickReadBooksSpec_OrdersByPagesAndThenTitle()
    {
        // Arrange
        var books = new List<Book>
        {
            Book.Create("Book C", "ISBN-1", 2020, 100, "Fiction", Guid.NewGuid()),
            Book.Create("Book A", "ISBN-2", 2021, 100, "Fiction", Guid.NewGuid()),
            Book.Create("Book B", "ISBN-3", 2022, 100, "Fiction", Guid.NewGuid()),
        };

        var spec = new QuickReadBooksSpec();

        // Act
        var result = spec.Evaluate(books).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        // All have same pages, so should be ordered by title
        Assert.Equal("Book A", result[0].Title);
        Assert.Equal("Book B", result[1].Title);
        Assert.Equal("Book C", result[2].Title);
    }
}
