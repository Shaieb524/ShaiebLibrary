using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShaiebLibrary.BookCatalog.Domain.Interfaces;
using ShaiebLibrary.BookCatalog.Infrastructure.Data;
using ShaiebLibrary.BookCatalog.Infrastructure.Repositories;

namespace ShaiebLibrary.BookCatalog.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Add DbContext
        services.AddDbContext<BookCatalogContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("ShaiebLibrary.BookCatalog.Infrastructure")
            ));

        // Register Repositories
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<IAuthorRepository, AuthorRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IPublisherRepository, PublisherRepository>();

        return services;
    }
}