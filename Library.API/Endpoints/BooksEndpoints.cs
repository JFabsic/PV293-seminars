using Library.BusinessLayer.Dtos;
using Library.BusinessLayer.Services;

namespace Library.API.Endpoints;

public static class BooksEndpoints
{
    public static void MapBooksEndpoints(this WebApplication app)
    {
        var books = app.MapGroup("/api/books")
            .WithTags("Books")
            .WithOpenApi();

        books.MapGet("", GetBooks)
            .WithName("GetBooks")
            .Produces<List<BookDto>>();

        books.MapPost("", CreateBook)
            .WithName("CreateBook")
            .Accepts<BookDto>("application/json")
            .Produces<BookDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem();
    }

    private static async Task<IResult> GetBooks(IBookService bookService)
    {
        var books = await bookService.GetAllBooksAsync();
        return Results.Ok(books);
    }

    private static async Task<IResult> CreateBook(BookDto bookDto, IBookService bookService)
    {
        try
        {
            var createdBook = await bookService.CreateBookAsync(bookDto);
            return Results.Created($"/api/books/{createdBook.Id}", createdBook);
        }
        catch (ArgumentException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { ex.ParamName ?? "Error", [ex.Message] },
            });
        }
    }
}