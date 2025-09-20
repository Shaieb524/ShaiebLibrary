using AutoMapper;

namespace ShaiebLibrary.BookCatalog.Application.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // This profile can include all the individual profile configurations
        // Or we can use the individual profiles separately
        
        // Include all other profiles
        // The individual profiles will be automatically discovered by AutoMapper
    }
}