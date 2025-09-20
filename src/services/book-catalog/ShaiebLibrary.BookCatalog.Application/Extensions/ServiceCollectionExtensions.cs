using Microsoft.Extensions.DependencyInjection;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;
using System.Reflection;

namespace ShaiebLibrary.BookCatalog.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Service registrations will be added here when we implement the services
        // services.AddScoped<IBookService, BookService>();
        // services.AddScoped<IAuthorService, AuthorService>();
        // services.AddScoped<ICategoryService, CategoryService>();
        // services.AddScoped<IPublisherService, PublisherService>();

        // Add AutoMapper with all profiles from this assembly
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        return services;
    }
}