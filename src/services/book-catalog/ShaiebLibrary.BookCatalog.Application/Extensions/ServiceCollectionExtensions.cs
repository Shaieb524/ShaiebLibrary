using Microsoft.Extensions.DependencyInjection;
using ShaiebLibrary.BookCatalog.Application.Services.Interfaces;

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

        // Add AutoMapper
        services.AddAutoMapper(typeof(ServiceCollectionExtensions));

        return services;
    }
}