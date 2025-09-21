using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ShaiebLibrary.BookCatalog.Application.DTOs.Book;
using ShaiebLibrary.BookCatalog.Application.Mapping;
using ShaiebLibrary.BookCatalog.Application.Services.Implementations;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Enums;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;

namespace ShaiebLibrary.BookCatalog.Tests.Services;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _mockBookRepository;
    private readonly Mock<IAuthorRepository> _mockAuthorRepository;
    private readonly Mock<ICategoryRepository> _mockCategoryRepository;
    private readonly Mock<IPublisherRepository> _mockPublisherRepository;
    private readonly IMapper _mapper;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        // Setup mocks
        _mockBookRepository = new Mock<IBookRepository>();
        _mockAuthorRepository = new Mock<IAuthorRepository>();
        _mockCategoryRepository = new Mock<ICategoryRepository>();
        _mockPublisherRepository = new Mock<IPublisherRepository>();

        // Setup AutoMapper
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<BookMappingProfile>();
            cfg.AddProfile<AuthorMappingProfile>();
            cfg.AddProfile<CategoryMappingProfile>();
            cfg.AddProfile<PublisherMappingProfile>();
        });
        _mapper = configuration.CreateMapper();

        // Create service instance
        _bookService = new BookService(
            _mockBookRepository.Object,
            _mockAuthorRepository.Object,
            _mockCategoryRepository.Object,
            _mockPublisherRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task GetBookByIdAsync_WhenBookExists_ReturnsBookDto()
    {
        // Arrange
        var bookId = 1;
        var book = CreateSampleBook(bookId);
        _mockBookRepository.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(book);

        // Act
        var result = await _bookService.GetBookByIdAsync(bookId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(bookId);
        result.Title.Should().Be("Test Book");
        result.ISBN.Should().Be("1234567890");
    }

    [Fact]
    public async Task GetBookByIdAsync_WhenBookDoesNotExist_ReturnsNull()
    {
        // Arrange
        var bookId = 999;
        _mockBookRepository.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.GetBookByIdAsync(bookId);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task CreateBookAsync_WithValidDto_ReturnsCreatedBook()
    {
        // Arrange
        var createBookDto = new CreateBookDto
        {
            Title = "New Book",
            ISBN = "9876543210",
            Language = Language.English,
            PublishedDate = DateTime.UtcNow.AddYears(-1),
            Pages = 300,
            Quantity = 2,
            AuthorIds = new List<int> { 1 },
            CategoryIds = new List<int> { 1 }
        };

        var createdBook = CreateSampleBook(1);
        createdBook.Title = createBookDto.Title;
        createdBook.ISBN = createBookDto.ISBN;

        _mockBookRepository.Setup(r => r.GetBookByISBNAsync(createBookDto.ISBN))
            .ReturnsAsync((Book?)null); // ISBN is unique

        _mockBookRepository.Setup(r => r.AddAsync(It.IsAny<Book>()))
            .ReturnsAsync(createdBook);

        _mockBookRepository.Setup(r => r.GetBookWithDetailsAsync(createdBook.Id))
            .ReturnsAsync(createdBook);

        // Act
        var result = await _bookService.CreateBookAsync(createBookDto);

        // Assert
        result.Should().NotBeNull();
        result.Title.Should().Be(createBookDto.Title);
        result.ISBN.Should().Be(createBookDto.ISBN);
        result.Status.Should().Be(BookStatus.Available);

        // Verify repository calls
        _mockBookRepository.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Once);
    }

    [Fact]
    public async Task CreateBookAsync_WithDuplicateISBN_ThrowsInvalidOperationException()
    {
        // Arrange
        var createBookDto = new CreateBookDto
        {
            Title = "New Book",
            ISBN = "1234567890", // Duplicate ISBN
            Language = Language.English,
            PublishedDate = DateTime.UtcNow,
            Pages = 300,
            Quantity = 1
        };

        var existingBook = CreateSampleBook(1);
        _mockBookRepository.Setup(r => r.GetBookByISBNAsync(createBookDto.ISBN))
            .ReturnsAsync(existingBook); // ISBN already exists

        // Act & Assert
        await _bookService.Invoking(s => s.CreateBookAsync(createBookDto))
            .Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");

        // Verify AddAsync was never called
        _mockBookRepository.Verify(r => r.AddAsync(It.IsAny<Book>()), Times.Never);
    }

    [Fact]
    public async Task IsBookAvailableAsync_WhenBookIsAvailable_ReturnsTrue()
    {
        // Arrange
        var bookId = 1;
        var book = CreateSampleBook(bookId);
        book.Status = BookStatus.Available;
        book.AvailableQuantity = 1;

        _mockBookRepository.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(book);

        // Act
        var result = await _bookService.IsBookAvailableAsync(bookId);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsBookAvailableAsync_WhenBookIsCheckedOut_ReturnsFalse()
    {
        // Arrange
        var bookId = 1;
        var book = CreateSampleBook(bookId);
        book.Status = BookStatus.CheckedOut;
        book.AvailableQuantity = 0;

        _mockBookRepository.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync(book);

        // Act
        var result = await _bookService.IsBookAvailableAsync(bookId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsBookAvailableAsync_WhenBookDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var bookId = 999;
        _mockBookRepository.Setup(r => r.GetByIdAsync(bookId))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.IsBookAvailableAsync(bookId);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateBookAvailabilityAsync_WithValidBookId_ReturnsTrue()
    {
        // Arrange
        var bookId = 1;
        var newQuantity = 5;

        _mockBookRepository.Setup(r => r.UpdateBookAvailabilityAsync(bookId, newQuantity))
            .ReturnsAsync(true);

        // Act
        var result = await _bookService.UpdateBookAvailabilityAsync(bookId, newQuantity);

        // Assert
        result.Should().BeTrue();
        _mockBookRepository.Verify(r => r.UpdateBookAvailabilityAsync(bookId, newQuantity), Times.Once);
    }

    [Fact]
    public async Task IsISBNUniqueAsync_WithNewISBN_ReturnsTrue()
    {
        // Arrange
        var isbn = "9999999999";
        _mockBookRepository.Setup(r => r.GetBookByISBNAsync(isbn))
            .ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.IsISBNUniqueAsync(isbn);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsISBNUniqueAsync_WithExistingISBN_ReturnsFalse()
    {
        // Arrange
        var isbn = "1234567890";
        var existingBook = CreateSampleBook(1);
        _mockBookRepository.Setup(r => r.GetBookByISBNAsync(isbn))
            .ReturnsAsync(existingBook);

        // Act
        var result = await _bookService.IsISBNUniqueAsync(isbn);

        // Assert
        result.Should().BeFalse();
    }

    // Helper method to create sample book
    private static Book CreateSampleBook(int id)
    {
        return new Book
        {
            Id = id,
            Title = "Test Book",
            ISBN = "1234567890",
            Language = Language.English,
            PublishedDate = DateTime.UtcNow.AddYears(-1),
            Pages = 250,
            Status = BookStatus.Available,
            Quantity = 1,
            AvailableQuantity = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            BookAuthors = new List<BookAuthor>(),
            BookCategories = new List<BookCategory>()
        };
    }
}