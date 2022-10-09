using DataAccess.Relational;
using Helpers.Migrations;

namespace Migrations;

/// <summary>
///     Startup
/// </summary>
/// <seealso cref="Helpers.Migrations.MigrationStartup{DbServiceContext}" />
public class Startup : MigrationStartup<DbServiceContext>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="Startup" /> class.
    /// </summary>
    /// <param name="env">The env.</param>
    /// <param name="configuration"></param>
    public Startup(IHostEnvironment env, IConfiguration configuration)
        : base(env, configuration)
    {
    }

    // This is required here!
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
    }
}