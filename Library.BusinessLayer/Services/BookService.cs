using Library.BusinessLayer.Dtos;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Library.DataAccess.UnitOfWork;

namespace Library.BusinessLayer.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IAuthorRepository _authorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public BookService(
        IBookRepository bookRepository,
        IAuthorRepository authorRepository,
        IUnitOfWork unitOfWork)
    {
        _bookRepository = bookRepository;
        _authorRepository = authorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<BookDto>> GetAllBooksAsync()
    {
        // What's the issue here? How would you refactor that?
        var books = await _bookRepository.GetAllAsync();
        var bookDtos = new List<BookDto>();

        foreach (var book in books)
        {
            var author = await _authorRepository.GetByIdAsync(book.AuthorId);
            bookDtos.Add(MapToDto(book, author));
        }

        return bookDtos;
    }

    public async Task<BookDto> CreateBookAsync(BookDto bookDto)
    {
        // This method is doing too much - violating SRP
        // Could be split into command validator, command handler and event handlers

        if (string.IsNullOrWhiteSpace(bookDto.Title))
            throw new ArgumentException("Book title is required", nameof(bookDto.Title));

        if (bookDto.AuthorId <= 0)
            throw new ArgumentException("Valid author ID is required", nameof(bookDto.AuthorId));

        // 1. Validate author exists
        var author = await _authorRepository.GetByIdAsync(bookDto.AuthorId);
        if (author == null)
            throw new ArgumentException($"Author with ID {bookDto.AuthorId} not found", nameof(bookDto.AuthorId));

        // 2. Check for duplicate ISBN
        var existingBooks = await _bookRepository.GetAllAsync();
        if (existingBooks.Any(b => b.ISBN == bookDto.ISBN))
            throw new ArgumentException($"Book with ISBN {bookDto.ISBN} already exists", nameof(bookDto.ISBN));

        // 3. Create the book
        var book = MapToEntity(bookDto);
        _bookRepository.Add(book);

        // 4. Update author statistics
        // NOTE: This is a side effect that should be handled by a domain event
        // With MediatR, this would be: _mediator.Publish(new BookCreatedEvent(book.Id, author.Id));
        // - And a separate handler would update the author
        author.TotalBooksPublished++;
        author.LastPublishedDate = DateTime.UtcNow;

        // - Also update the author's most popular genre based on all their books
        var authorBooks = await _bookRepository.GetAllAsync();
        var authorBooksList = authorBooks.Where(b => b.AuthorId == author.Id).ToList();
        if (authorBooksList.Any())
        {
            var mostPopularGenre = authorBooksList
                .GroupBy(b => b.Genre)
                .OrderByDescending(g => g.Count())
                .Select(g => g.Key)
                .FirstOrDefault();

            if (!string.IsNullOrEmpty(mostPopularGenre))
            {
                author.MostPopularGenre = mostPopularGenre;
            }
        }

        _authorRepository.Update(author);

        // 5. Save all changes
        // NOTE: With MediatR pipeline behavior, this could be handled automatically
        // through a TransactionalBehavior that wraps all handlers in a transaction
        await _unitOfWork.CompleteAsync();

        return MapToDto(book, author);
    }

    private static BookDto MapToDto(Book book, Author? author = null)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            AuthorId = book.AuthorId,
            AuthorName = author?.Name ?? "Unknown",
            ISBN = book.ISBN,
            Year = book.Year,
            Pages = book.Pages,
            Genre = book.Genre,
        };
    }

    private static Book MapToEntity(BookDto bookDto)
    {
        return new Book
        {
            Id = bookDto.Id,
            Title = bookDto.Title,
            AuthorId = bookDto.AuthorId,
            ISBN = bookDto.ISBN,
            Year = bookDto.Year,
            Pages = bookDto.Pages,
            Genre = bookDto.Genre,
        };
    }
}