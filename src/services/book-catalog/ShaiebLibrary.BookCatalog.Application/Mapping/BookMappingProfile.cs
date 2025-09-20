using AutoMapper;
using ShaiebLibrary.BookCatalog.Application.DTOs.Book;
using ShaiebLibrary.BookCatalog.Domain.Entities;
using ShaiebLibrary.BookCatalog.Domain.Enums;

namespace ShaiebLibrary.BookCatalog.Application.Mapping;

public class BookMappingProfile : Profile
{
    public BookMappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<Book, BookDto>()
            .ForMember(dest => dest.Authors, opt => opt.MapFrom(src => 
                src.BookAuthors.Select(ba => ba.Author)))
            .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => 
                src.BookCategories.Select(bc => bc.Category)));

        // DTO to Entity mappings
        CreateMap<CreateBookDto, Book>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => BookStatus.Available))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.Quantity))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Publisher, opt => opt.Ignore())
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore())
            .ForMember(dest => dest.BookCategories, opt => opt.Ignore());

        CreateMap<UpdateBookDto, Book>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Publisher, opt => opt.Ignore())
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore())
            .ForMember(dest => dest.BookCategories, opt => opt.Ignore());
    }
}