using Library.BusinessLayer.Dtos;
using Library.DataAccess.Entities;
using Library.DataAccess.Repositories;
using Library.DataAccess.UnitOfWork;

namespace Library.BusinessLayer.Services;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AuthorService(IAuthorRepository authorRepository, IUnitOfWork unitOfWork)
    {
        _authorRepository = authorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<AuthorDto>> GetAllAuthorsAsync()
    {
        var authors = await _authorRepository.GetAllAsync();
        return authors.Select(MapToDto).ToList();
    }

    private static AuthorDto MapToDto(Author author)
    {
        return new AuthorDto
        {
            Id = author.Id,
            Name = author.Name,
            Biography = author.Biography,
            BirthDate = author.BirthDate,
            Country = author.Country,
            TotalBooksPublished = author.TotalBooksPublished,
            LastPublishedDate = author.LastPublishedDate,
            MostPopularGenre = author.MostPopularGenre
        };
    }
}