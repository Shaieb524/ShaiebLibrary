using Microsoft.EntityFrameworkCore;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Infrastructure.Data;

public class BookCatalogContext : DbContext
{
    public BookCatalogContext(DbContextOptions<BookCatalogContext> options) : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookAuthor> BookAuthors { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure entity relationships and constraints
        ConfigureBookEntity(modelBuilder);
        ConfigureAuthorEntity(modelBuilder);
        ConfigurePublisherEntity(modelBuilder);
        ConfigureCategoryEntity(modelBuilder);
        ConfigureBookAuthorEntity(modelBuilder);
        ConfigureBookCategoryEntity(modelBuilder);

        // Seed initial data
        SeedData(modelBuilder);
    }

    private static void ConfigureBookEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.Subtitle)
                .HasMaxLength(500);
                
            entity.Property(e => e.ISBN)
                .IsRequired()
                .HasMaxLength(13);
                
            entity.Property(e => e.ISBN13)
                .HasMaxLength(17);
                
            entity.Property(e => e.Language)
                .HasConversion<int>()
                .IsRequired();
                
            entity.Property(e => e.Status)
                .HasConversion<int>()
                .IsRequired();
                
            entity.Property(e => e.Description)
                .HasColumnType("NTEXT");
                
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10,2)");
                
            entity.Property(e => e.CoverImageUrl)
                .HasMaxLength(500);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            entity.HasIndex(e => e.ISBN).IsUnique();
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Language);
            entity.HasIndex(e => e.Status);

            // Relationships
            entity.HasOne(e => e.Publisher)
                .WithMany(p => p.Books)
                .HasForeignKey(e => e.PublisherId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }

    private static void ConfigureAuthorEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Biography)
                .HasColumnType("NTEXT");
                
            entity.Property(e => e.PhotoUrl)
                .HasMaxLength(500);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            entity.HasIndex(e => new { e.FirstName, e.LastName });
        });
    }

    private static void ConfigurePublisherEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Publisher>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.Address)
                .HasMaxLength(500);
                
            entity.Property(e => e.Website)
                .HasMaxLength(200);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            entity.HasIndex(e => e.Name);
        });
    }

    private static void ConfigureCategoryEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.Description)
                .HasMaxLength(500);
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
                
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Self-referencing relationship
            entity.HasOne(e => e.ParentCategory)
                .WithMany(e => e.SubCategories)
                .HasForeignKey(e => e.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            entity.HasIndex(e => e.Name);
        });
    }

    private static void ConfigureBookAuthorEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookAuthor>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.AuthorId });
            
            entity.Property(e => e.Role)
                .HasConversion<int>()
                .IsRequired();
                
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Book)
                .WithMany(b => b.BookAuthors)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Author)
                .WithMany(a => a.BookAuthors)
                .HasForeignKey(e => e.AuthorId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureBookCategoryEntity(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BookCategory>(entity =>
        {
            entity.HasKey(e => new { e.BookId, e.CategoryId });
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            entity.HasOne(e => e.Book)
                .WithMany(b => b.BookCategories)
                .HasForeignKey(e => e.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Category)
                .WithMany(c => c.BookCategories)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Seed Categories
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Fiction", Description = "Fictional literature", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Category { Id = 2, Name = "Non-Fiction", Description = "Non-fictional literature", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Category { Id = 3, Name = "Science", Description = "Scientific books", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Category { Id = 4, Name = "History", Description = "Historical books", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Category { Id = 5, Name = "Technology", Description = "Technology and programming books", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Category { Id = 6, Name = "Literature", Description = "Classic and modern literature", CreatedAt = seedDate, UpdatedAt = seedDate }
        );

        // Seed Publishers
        modelBuilder.Entity<Publisher>().HasData(
            new Publisher { Id = 1, Name = "Penguin Random House", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Publisher { Id = 2, Name = "HarperCollins", CreatedAt = seedDate, UpdatedAt = seedDate },
            new Publisher { Id = 3, Name = "O'Reilly Media", CreatedAt = seedDate, UpdatedAt = seedDate }
        );
    }
}