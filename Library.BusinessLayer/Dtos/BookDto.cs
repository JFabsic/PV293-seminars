namespace Library.BusinessLayer.Dtos;

public class BookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public int Year { get; set; }
    public int Pages { get; set; }
    public string Genre { get; set; } = string.Empty;

    // Author information
    public int AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
}