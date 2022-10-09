using Microsoft.EntityFrameworkCore;

namespace Helpers.DataAccess.Relational;

/// <summary>
///     Postgre SQL database context, overriding some DbContext methods
/// </summary>
/// <seealso cref="Microsoft.EntityFrameworkCore.DbContext" />
public class PostgreSqlDbContext : DbContext
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="PostgreSqlDbContext" /> class.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected PostgreSqlDbContext(DbContextOptions options)
        : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseSerialColumns();
    }

    /// <summary>
    ///     Performs HasPostgresExtension call for both Npgsql v.1.1.0 (netcore 1.1 - for migrations) and v.1.0.2 (netcore 1.0)
    /// </summary>
    protected void HasPostgresExtension(ModelBuilder modelBuilder, string extension)
    {
        modelBuilder.HasPostgresExtension(extension);
    }
}