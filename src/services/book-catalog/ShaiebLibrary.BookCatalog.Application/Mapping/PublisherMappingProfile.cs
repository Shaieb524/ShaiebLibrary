using AutoMapper;
using ShaiebLibrary.BookCatalog.Application.DTOs.Publisher;
using ShaiebLibrary.BookCatalog.Domain.Entities;

namespace ShaiebLibrary.BookCatalog.Application.Mapping;

public class PublisherMappingProfile : Profile
{
    public PublisherMappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<Publisher, PublisherDto>();

        // DTO to Entity mappings
        CreateMap<CreatePublisherDto, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Books, opt => opt.Ignore());

        // For updates
        CreateMap<CreatePublisherDto, Publisher>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Books, opt => opt.Ignore());
    }
}