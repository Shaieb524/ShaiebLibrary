using AutoMapper;
using ShaiebLibrary.BookCatalog.Application.DTOs.Common;

namespace ShaiebLibrary.BookCatalog.Application.Extensions;

public static class MappingExtensions
{
    public static PagedResult<TDestination> MapToPagedResult<TSource, TDestination>(
        this IMapper mapper,
        IEnumerable<TSource> source,
        int totalCount,
        int currentPage,
        int pageSize)
    {
        return new PagedResult<TDestination>
        {
            Data = mapper.Map<IEnumerable<TDestination>>(source),
            TotalCount = totalCount,
            CurrentPage = currentPage,
            PageSize = pageSize
        };
    }
}