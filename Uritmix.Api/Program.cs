using System.Globalization;
using AutoMapper;
using Command;
using DataAccess.Relational;
using Helpers.Mapping;
using Helpers.Security;
using Helpers.WebApi.ErrorHandling;
using Helpers.WebApi.Extensions;
using Mapping;
using Mapping.Person;
using Microsoft.AspNetCore.Localization;
using Serilog;
using Service.HostedService;
using Service.HostedService.InitAdmin;
using Service.Notification;
using Service.Notification.SendToken;
using Service.Security.PasswordHasher;
using Service.Security.UserAccessor;
using Service.Security.UserJwt;

const string appName = "Uritmix.Api";

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

var configuration = builder.Configuration; // allows both to access and to set up the config
var services = builder.Services;

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwagger(appName);
services.AddJwtAuth(configuration);
services.AddLocalization(options => options.ResourcesPath = "Resources");

// Add mediator
services.ConfigureServices(new List<Type>
{
    typeof(AddMediatR)
});

// AutoMapper
services.AddAutoMapper(new List<Profile>
{
    new PaginatedListProfile(),
    new MappingModelToView(),
    new MappingEntryToModel(),
    new Mapping.Room.MappingModelToView(),
    new Mapping.Room.MappingEntryToModel(),
    new Mapping.Lesson.MappingModelToView(),
    new Mapping.Lesson.MappingEntryToModel(),
    new Mapping.Abonnement.MappingModelToView(),
    new Mapping.Abonnement.MappingEntryToModel(),
    new Mapping.Event.MappingModelToView(),
    new Mapping.Event.MappingEntryToModel(),
});

// Configure db context
var dbConnectionString = configuration.GetConnectionString("DefaultConnection");
services.RegistrationDbContext(dbConnectionString);

// Configure options
services.OptionsRegistration<JwtOptions>(configuration, JwtOptions.Options);
services.OptionsRegistration<NotificationOptions>(configuration, NotificationOptions.Options);
services.OptionsRegistration<InitAdminOptions>(configuration, InitAdminOptions.Options);

// Configure services
services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
services.AddScoped<IUserAccessor, UserAccessor>();
services.AddScoped<ISendToken, SendToken>();

services.AddHostedService<ConsumeScopedServiceHostedService>();
services.AddScoped<IInitAdminService, InitAdminService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", appName));
}

var supportedCultures = new[]
{
    new CultureInfo("en")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseMiddleware<ErrorHandlingMiddleware>();

app.AddSerilogLogging();
// app.UseHttpsRedirection();
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();
app.Run();