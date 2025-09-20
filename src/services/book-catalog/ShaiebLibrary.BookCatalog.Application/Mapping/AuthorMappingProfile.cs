using AutoMapper;
using ShaiebLibrary.BookCatalog.Application.DTOs.Author;
using ShaiebLibrary.BookCatalog.Domain.Entities;

namespace ShaiebLibrary.BookCatalog.Application.Mapping;

public class AuthorMappingProfile : Profile
{
    public AuthorMappingProfile()
    {
        // Entity to DTO mappings
        CreateMap<Author, AuthorDto>()
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName));

        // DTO to Entity mappings
        CreateMap<CreateAuthorDto, Author>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore());

        // For updates, we can reuse CreateAuthorDto
        CreateMap<CreateAuthorDto, Author>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.BookAuthors, opt => opt.Ignore());
    }
}