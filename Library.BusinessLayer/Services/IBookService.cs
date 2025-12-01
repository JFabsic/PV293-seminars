using Library.BusinessLayer.Dtos;

namespace Library.BusinessLayer.Services;

public interface IBookService
{
    Task<List<BookDto>> GetAllBooksAsync();
    Task<BookDto> CreateBookAsync(BookDto bookDto);
}