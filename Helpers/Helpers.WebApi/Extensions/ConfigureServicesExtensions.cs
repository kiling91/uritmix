using System.Reflection;
using FluentValidation;
using Helpers.WebApi.Validator;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Helpers.WebApi.Extensions;

public static class ConfigureServicesExtensions
{
    private static IList<JsonConverter> JsonConverters()
    {
        var converters = new List<JsonConverter>
        {
            new StringEnumConverter()
        };
        return converters;
    }

    public static void ConfigureServices(this IServiceCollection services, IEnumerable<Type> assemblyTypes)
    {
        JsonConvert.DefaultSettings = () => new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = JsonConverters()
        };

        services.AddTransient(typeof(IPipelineBehavior<,>),
            typeof(ValidationPipelineBehavior<,>));

        foreach (var type in assemblyTypes)
            services.AddValidatorsFromAssemblyContaining(type);

        ValidatorOptions.Global.PropertyNameResolver = CamelCasePropertyNameResolver.ResolvePropertyName;


        services.AddMvc(opt => { opt.EnableEndpointRouting = false; })
            .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    options.SerializerSettings.Converters = JsonConverters();
                }
            );

        services.AddControllers()
            .ConfigureApiBehaviorOptions(options => { options.SuppressModelStateInvalidFilter = true; });


        if (assemblyTypes.Any())
            services.AddMediatR(assemblyTypes.Select(t => t.GetTypeInfo().Assembly).ToArray());
    }
}