using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Helpers.WebApi.Extensions;

public static class OptionsRegistrationExtensions
{
    public static void OptionsRegistration<TOptions>(
        this IServiceCollection services,
        IConfiguration configuration,
        string optionsName)
        where TOptions : class
    {
        services.Configure<TOptions>(configuration.GetSection(optionsName));
    }
}