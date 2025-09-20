using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Application.DTOs.Book;

public class BookSearchCriteria
{
    public string? SearchTerm { get; set; }
    public Language? Language { get; set; }
    public BookStatus? Status { get; set; }
    public int? AuthorId { get; set; }
    public int? CategoryId { get; set; }
    public int? PublisherId { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}