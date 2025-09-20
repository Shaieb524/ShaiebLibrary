using ShaiebLibrary.BookCatalog.Domain.Entities;

namespace ShaiebLibrary.BookCatalog.Domain.Interfaces;

public interface IAuthorRepository : IRepository<Author>
{
    Task<IEnumerable<Author>> SearchAuthorsByNameAsync(string searchTerm);
    Task<Author?> GetAuthorByFullNameAsync(string firstName, string lastName);
    Task<IEnumerable<Author>> GetAuthorsByBookAsync(int bookId);
    Task<Author?> GetAuthorWithBooksAsync(int id);
}