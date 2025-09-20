using ShaiebLibrary.BookCatalog.Application.DTOs.Common;

namespace ShaiebLibrary.BookCatalog.Application.DTOs.Author;

public class AuthorDto : BaseDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public DateTime? BirthDate { get; set; }
    public DateTime? DeathDate { get; set; }
    public string? PhotoUrl { get; set; }
}   