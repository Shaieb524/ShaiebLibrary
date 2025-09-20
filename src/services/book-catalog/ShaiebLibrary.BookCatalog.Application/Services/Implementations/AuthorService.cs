using AutoMapper;
using ShaiebLibrary.BookCatalog.Application.DTOs.Author;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;
using ShaiebLibrary.BookCatalog.Application.Extensions;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;

namespace ShaiebLibrary.BookCatalog.Application.Services.Implementations;

public class AuthorService : IAuthorService
{
    private readonly IAuthorRepository _authorRepository;
    private readonly IMapper _mapper;

    public AuthorService(IAuthorRepository authorRepository, IMapper mapper)
    {
        _authorRepository = authorRepository;
        _mapper = mapper;
    }

    public async Task<AuthorDto?> GetAuthorByIdAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        return author == null ? null : _mapper.Map<AuthorDto>(author);
    }

    public async Task<AuthorDto?> GetAuthorWithBooksAsync(int id)
    {
        var author = await _authorRepository.GetAuthorWithBooksAsync(id);
        return author == null ? null : _mapper.Map<AuthorDto>(author);
    }

    public async Task<PagedResult<AuthorDto>> GetAuthorsAsync(int page = 1, int pageSize = 10)
    {
        var allAuthors = await _authorRepository.GetAllAsync();
        var totalCount = allAuthors.Count();
        var authors = allAuthors.Skip((page - 1) * pageSize).Take(pageSize);

        return _mapper.MapToPagedResult<Author, AuthorDto>(authors, totalCount, page, pageSize);
    }

    public async Task<AuthorDto> CreateAuthorAsync(CreateAuthorDto createAuthorDto)
    {
        var author = _mapper.Map<Author>(createAuthorDto);
        var createdAuthor = await _authorRepository.AddAsync(author);
        return _mapper.Map<AuthorDto>(createdAuthor);
    }

    public async Task<AuthorDto?> UpdateAuthorAsync(int id, CreateAuthorDto updateAuthorDto)
    {
        var existingAuthor = await _authorRepository.GetByIdAsync(id);
        if (existingAuthor == null)
            return null;

        _mapper.Map(updateAuthorDto, existingAuthor);
        existingAuthor.UpdatedAt = DateTime.UtcNow;
        
        await _authorRepository.UpdateAsync(existingAuthor);
        return _mapper.Map<AuthorDto>(existingAuthor);
    }

    public async Task<bool> DeleteAuthorAsync(int id)
    {
        var author = await _authorRepository.GetByIdAsync(id);
        if (author == null)
            return false;

        await _authorRepository.DeleteAsync(author);
        return true;
    }

    public async Task<IEnumerable<AuthorDto>> SearchAuthorsByNameAsync(string searchTerm)
    {
        var authors = await _authorRepository.SearchAuthorsByNameAsync(searchTerm);
        return _mapper.Map<IEnumerable<AuthorDto>>(authors);
    }

    public async Task<AuthorDto?> GetAuthorByFullNameAsync(string firstName, string lastName)
    {
        var author = await _authorRepository.GetAuthorByFullNameAsync(firstName, lastName);
        return author == null ? null : _mapper.Map<AuthorDto>(author);
    }

    public async Task<IEnumerable<AuthorDto>> GetAuthorsByBookAsync(int bookId)
    {
        var authors = await _authorRepository.GetAuthorsByBookAsync(bookId);
        return _mapper.Map<IEnumerable<AuthorDto>>(authors);
    }

    public async Task<bool> AuthorExistsAsync(int id)
    {
        return await _authorRepository.ExistsAsync(id);
    }

    public async Task<AuthorDto?> GetOrCreateAuthorAsync(string firstName, string lastName)
    {
        var existingAuthor = await _authorRepository.GetAuthorByFullNameAsync(firstName, lastName);
        
        if (existingAuthor != null)
        {
            return _mapper.Map<AuthorDto>(existingAuthor);
        }

        var createAuthorDto = new CreateAuthorDto
        {
            FirstName = firstName,
            LastName = lastName
        };

        return await CreateAuthorAsync(createAuthorDto);
    }
}