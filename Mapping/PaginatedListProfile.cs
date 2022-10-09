using AutoMapper;
using Helpers.Pagination;

namespace Mapping;

/// <summary>
///     Paginated list profile
/// </summary>
/// <seealso cref="AutoMapper.Profile" />
public class PaginatedListProfile : Profile
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PaginatedListProfile" /> class.
    /// </summary>
    public PaginatedListProfile()
    {
        CreateMap(typeof(PaginatedList<>), typeof(PaginatedList<>))
            .ForMember("Results", opt => { opt.MapFrom(l => l); });

        CreateMap(typeof(PaginatedList<>), typeof(PaginatedListViewModel<>))
            .ForMember("Results", opt => { opt.MapFrom(l => l); });
    }
}