using Library.DataAccess.Data;
using Library.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Repositories;

public class AuthorRepository(ApplicationDbContext context) : Repository<Author>(context), IAuthorRepository
{
    private ApplicationDbContext ApplicationDbContext => (ApplicationDbContext)Context;
}