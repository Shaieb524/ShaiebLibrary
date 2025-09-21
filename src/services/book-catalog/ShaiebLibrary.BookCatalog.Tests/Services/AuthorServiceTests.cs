using AutoMapper;
using FluentAssertions;
using Moq;
using ShaiebLibrary.BookCatalog.Application.DTOs.Author;
using ShaiebLibrary.BookCatalog.Application.Mapping;
using ShaiebLibrary.BookCatalog.Application.Services.Implementations;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;

namespace ShaiebLibrary.BookCatalog.Tests.Services;

public class AuthorServiceTests
{
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;
    private readonly IMapper _mapper;
    private readonly AuthorService _authorService;

    public AuthorServiceTests()
    {
        _mockAuthorRepository = new Mock<IAuthorRepository>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AuthorMappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        _authorService = new AuthorService(_mockAuthorRepository.Object, _mapper);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_WhenAuthorExists_ReturnsAuthorDto()
    {
        // Arrange
        var authorId = 1;
        var author = CreateSampleAuthor(authorId);
        _mockAuthorRepository.Setup(r => r.GetByIdAsync(authorId))
            .ReturnsAsync(author);

        // Act
        var result = await _authorService.GetAuthorByIdAsync(authorId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(authorId);
        result.FirstName.Should().Be("John");
        result.LastName.Should().Be("Doe");
        result.FullName.Should().Be("John Doe");
    }

    [Fact]
    public async Task CreateAuthorAsync_WithValidDto_ReturnsCreatedAuthor()
    {
        // Arrange
        var createAuthorDto = new CreateAuthorDto
        {
            FirstName = "Jane",
            LastName = "Smith",
            Biography = "A great author"
        };

        var createdAuthor = new Author
        {
            Id = 1,
            FirstName = createAuthorDto.FirstName,
            LastName = createAuthorDto.LastName,
            Biography = createAuthorDto.Biography,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockAuthorRepository.Setup(r => r.AddAsync(It.IsAny<Author>()))
            .ReturnsAsync(createdAuthor);

        // Act
        var result = await _authorService.CreateAuthorAsync(createAuthorDto);

        // Assert
        result.Should().NotBeNull();
        result.FirstName.Should().Be(createAuthorDto.FirstName);
        result.LastName.Should().Be(createAuthorDto.LastName);
        result.FullName.Should().Be("Jane Smith");

        _mockAuthorRepository.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Once);
    }

    [Fact]
    public async Task GetOrCreateAuthorAsync_WhenAuthorExists_ReturnsExistingAuthor()
    {
        // Arrange
        var firstName = "John";
        var lastName = "Doe";
        var existingAuthor = CreateSampleAuthor(1);

        _mockAuthorRepository.Setup(r => r.GetAuthorByFullNameAsync(firstName, lastName))
            .ReturnsAsync(existingAuthor);

        // Act
        var result = await _authorService.GetOrCreateAuthorAsync(firstName, lastName);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        result.FirstName.Should().Be(firstName);
        result.LastName.Should().Be(lastName);

        // Verify AddAsync was never called
        _mockAuthorRepository.Verify(r => r.AddAsync(It.IsAny<Author>()), Times.Never);
    }

    [Fact]
    public async Task AuthorExistsAsync_WhenAuthorExists_ReturnsTrue()
    {
        // Arrange
        var authorId = 1;
        _mockAuthorRepository.Setup(r => r.ExistsAsync(authorId))
            .ReturnsAsync(true);

        // Act
        var result = await _authorService.AuthorExistsAsync(authorId);

        // Assert
        result.Should().BeTrue();
    }

    // Helper method
    private static Author CreateSampleAuthor(int id)
    {
        return new Author
        {
            Id = id,
            FirstName = "John",
            LastName = "Doe",
            Biography = "Test author biography",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookAuthors = new List<BookAuthor>()
        };
    }
}