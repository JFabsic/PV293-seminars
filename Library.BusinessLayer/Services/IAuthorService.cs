using Library.BusinessLayer.Dtos;

namespace Library.BusinessLayer.Services;

public interface IAuthorService
{
    Task<List<AuthorDto>> GetAllAuthorsAsync();
}