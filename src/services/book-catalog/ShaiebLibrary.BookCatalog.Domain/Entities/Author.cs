namespace ShaiebLibrary.BookCatalog.Domain.Entities;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? PhotoUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    // Navigation Properties
    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();
    
    // Computed Property
    public string FullName => $"{FirstName} {LastName}";
}