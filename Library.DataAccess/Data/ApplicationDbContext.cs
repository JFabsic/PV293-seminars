using Library.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Library.DataAccess.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Author entity
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Biography).HasMaxLength(1000);
            entity.Property(e => e.Country).HasMaxLength(50);
            entity.Property(e => e.MostPopularGenre).HasMaxLength(50);
        });

        // Configure Book entity
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ISBN).HasMaxLength(20);
            entity.Property(e => e.Genre).HasMaxLength(50);

            // Configure relationship
            entity.HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed data for Authors
        modelBuilder.Entity<Author>().HasData(
            new Author
            {
                Id = 1,
                Name = "Robert C. Martin",
                Biography = "Software engineer and author, known for Clean Code",
                BirthDate = new DateTime(1952, 12, 5),
                Country = "USA",
                TotalBooksPublished = 1,
                MostPopularGenre = "Programming"
            },
            new Author
            {
                Id = 2,
                Name = "Gang of Four",
                Biography = "Erich Gamma, Richard Helm, Ralph Johnson, and John Vlissides",
                BirthDate = new DateTime(1960, 1, 1),
                Country = "Various",
                TotalBooksPublished = 1,
                MostPopularGenre = "Programming"
            },
            new Author
            {
                Id = 3,
                Name = "David Thomas & Andrew Hunt",
                Biography = "Authors of The Pragmatic Programmer",
                BirthDate = new DateTime(1965, 1, 1),
                Country = "USA",
                TotalBooksPublished = 1,
                MostPopularGenre = "Programming"
            }
        );

        // Update seed data for Books with AuthorId
        modelBuilder.Entity<Book>().HasData(
            new Book { Id = 1, Title = "Clean Code", AuthorId = 1, ISBN = "978-0132350884", Year = 2008, Pages = 464, Genre = "Programming" },
            new Book { Id = 2, Title = "The Pragmatic Programmer", AuthorId = 3, ISBN = "978-0135957059", Year = 2019, Pages = 352, Genre = "Programming" },
            new Book { Id = 3, Title = "Design Patterns", AuthorId = 2, ISBN = "978-0201633610", Year = 1994, Pages = 395, Genre = "Programming" }
        );
    }
}