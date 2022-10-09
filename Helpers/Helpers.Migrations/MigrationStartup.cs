using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Helpers.Migrations;

/// <summary>
///     Base class for migration startup
/// </summary>
public abstract class MigrationStartup<TContext> where TContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="MigrationStartup{TContext}" /> class.
    /// </summary>
    /// <param name="env">The env.</param>
    protected MigrationStartup(IHostEnvironment env, IConfiguration configuration)
    {
        HostEnvironment = env;
        Configuration = configuration;
    }

    protected IHostEnvironment HostEnvironment { get; }

    /// <summary>
    ///     Gets the configuration.
    /// </summary>
    protected IConfiguration Configuration { get; }

    /// <summary>
    ///     This method gets called by the runtime. Use this method to add services to the container
    /// </summary>
    /// <param name="services">The services.</param>
    public virtual void ConfigureServices(IServiceCollection services)
    {
        ////ЮМС: Разремить при отладке миграций. После:
        ////PM> $env:ASPNETCORE_ENVIRONMENT="Development"
        ////PM> Update-Database
        ////Выбрать отладчик
        ////Отладить
        ////PM> $env:ASPNETCORE_ENVIRONMENT="Staging"
        //if (System.Diagnostics.Debugger.IsAttached == false)
        //    System.Diagnostics.Debugger.Launch();

        RegisterDbContext(services);
    }

    /// <summary>
    ///     This method gets called by the runtime. Use this method to configure the HTTP request pipeline
    /// </summary>
    /// <param name="app">The application.</param>
    /// <param name="env">The env.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public virtual void Configure(IApplicationBuilder app, IHostEnvironment env, ILoggerFactory loggerFactory)
    {
    }

    protected virtual void RegisterDbContext(IServiceCollection services)
    {
        var dbConnectionString = GetConnectionString();
        Console.WriteLine("Migration assembly: " + GetType().GetTypeInfo().Assembly.GetName().Name);
        Console.WriteLine("Connection string: " + dbConnectionString);
        services.AddDbContext<TContext>(options =>
            options.UseNpgsql(dbConnectionString, b =>
                b.MigrationsAssembly(GetType().GetTypeInfo().Assembly.GetName().Name)));
    }

    protected virtual string GetConnectionString()
    {
        return Configuration.GetConnectionString("DefaultConnection");
    }
}