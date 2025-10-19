using Library.Domain.Common;
using Library.Domain.DomainEvents;

namespace Library.Domain.Aggregates;

public class Book : AggregateRoot
{
    public string Title { get; private set; } = string.Empty;
    public string ISBN { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public int Pages { get; private set; }
    public string Genre { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public bool IsAvailable { get; private set; }

    // Foreign key
    public Guid AuthorId { get; private set; }

    // Navigation property
    public Author Author { get; set; } = null!;

    // Private constructor for EF Core
    private Book() : base()
    {
    }

    // Constructor with ID for seeding
    private Book(Guid id) : base(id)
    {
    }

    // Factory method for creating a new book
    public static Book Create(string title, string isbn, int year, int pages, string genre, Guid authorId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (authorId == Guid.Empty)
            throw new ArgumentException("Author ID cannot be empty", nameof(authorId));

        var book = new Book
        {
            Title = title,
            ISBN = isbn,
            Year = year,
            Pages = pages,
            Genre = genre,
            AuthorId = authorId,
            CreatedAt = DateTime.UtcNow,
            IsAvailable = true, // New books are available by default
        };

        book.RaiseDomainEvent(new BookCreated
        {
            BookId = book.Id,
            AuthorId = book.AuthorId,
            Genre = book.Genre,
        });

        return book;
    }

    public void UpdateDetails(string title, int year, int pages, string genre)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        Title = title;
        Year = year;
        Pages = pages;
        Genre = genre;
    }

    // TODO: these methods should be called whenever the book is on loan
    public void MarkAsAvailable()
    {
        IsAvailable = true;
    }

    public void UnmarkAsLoaned()
    {
        IsAvailable = false;
    }

    public bool IsClassic() => Year < 1950;
    public bool IsNewRelease() => Year >= DateTime.Now.Year - 2;

    public TimeSpan GetEstimatedReadingTime()
    {
        const int wordsPerPage = 250;
        const int wordsPerMinute = 200;
        var totalWords = Pages * wordsPerPage;
        var minutes = totalWords / wordsPerMinute;
        return TimeSpan.FromMinutes(minutes);
    }
}