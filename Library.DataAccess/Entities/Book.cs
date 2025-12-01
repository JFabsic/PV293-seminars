namespace Library.DataAccess.Entities;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Pages { get; set; }
    public string Genre { get; set; } = string.Empty;

    // Foreign key
    public int AuthorId { get; set; }
    // Navigation property
    public Author Author { get; set; } = null!;
}