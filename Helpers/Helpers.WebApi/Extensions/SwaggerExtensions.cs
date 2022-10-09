using System.ComponentModel;
using System.Reflection;
using Helpers.Core;
using Helpers.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Helpers.WebApi.Extensions;

public class CustomModelDocumentFilter<T> : IDocumentFilter where T : class
{
    public void Apply(OpenApiDocument openapiDoc, DocumentFilterContext context)
    {
        context.SchemaGenerator.GenerateSchema(typeof(T), context.SchemaRepository);
    }
}

public static class SwaggerExtensions
{
    private static string GetDisplayName(Type t)
    {
        var attribute = t.GetCustomAttributes<DisplayNameAttribute>().SingleOrDefault();
        return attribute?.DisplayName ?? t.Name;
    }

    private static string RenameName(Type t)
    {
        if (t.Name == typeof(ResultResponse<>).Name)
        {
            var type = t.GetGenericArguments()[0];

            if (type.Name == typeof(PaginatedListViewModel<>).Name)
            {
                type = type.GetGenericArguments()[0];
                return "ResultPaginated" + GetDisplayName(type);
            }

            return "Result" + GetDisplayName(type);
        }

        if (t.Name == typeof(PaginatedListViewModel<>).Name)
        {
            var type = t.GetGenericArguments()[0];
            return "Paginated" + GetDisplayName(type);
        }

        return GetDisplayName(t);
    }

    public static void AddSwagger(this IServiceCollection services, string appName)
    {
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddSwaggerGen(swagger =>
        {
            swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            swagger.OperationFilter<AuthOperationFilter>();
            swagger.SupportNonNullableReferenceTypes();

            swagger.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = appName,
                Version = "v1"
            });

            swagger.DocumentFilter<CustomModelDocumentFilter<ErrorResponse>>();
            swagger.DocumentFilter<CustomModelDocumentFilter<ValidError>>();
            swagger.DocumentFilter<CustomModelDocumentFilter<PropertyError>>();

            swagger.CustomSchemaIds(RenameName);
            swagger.DescribeAllParametersInCamelCase();

            swagger.MapType<ProblemDetails>(() => new OpenApiSchema
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.Schema,
                    Id = nameof(Unit)
                }
            });

            swagger.DocInclusionPredicate((_, _) => true);
            var xmlFile = $"{appName}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            swagger.IncludeXmlComments(xmlPath);
        });
    }
}