using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.Mapping;

public static class AutoMapperExtensions
{
    public static void AddAutoMapper(this IServiceCollection services, IEnumerable<Profile> profiles)
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            //mc.AddProfile(new PaginatedListProfile());
            foreach (var profile in profiles)
                mc.AddProfile(profile);
        });
        mapperConfig.AssertConfigurationIsValid();
        var mapper = mapperConfig.CreateMapper();
        services.AddSingleton(mapper);
    }
}