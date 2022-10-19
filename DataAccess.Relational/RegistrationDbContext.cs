using DataAccess.Abonnement;
using DataAccess.Auth;
using DataAccess.Event;
using DataAccess.Lesson;
using DataAccess.Relational.Abonnement;
using DataAccess.Relational.Auth;
using DataAccess.Relational.Event;
using DataAccess.Relational.Lesson;
using DataAccess.Relational.Room;
using DataAccess.Room;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Relational;

public static class RegistrationDbContextExtension
{
    public static void RegistrationDbContext(this IServiceCollection services, string dbConnectionString)
    {
        services.AddScoped<IPersonRepository, PersonRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IConfirmationCoderRepository, ConfirmationCoderRepository>();
        services.AddScoped<IRoomRepository, RoomRepository>();
        services.AddScoped<ILessonRepository, LessonRepository>();
        services.AddScoped<IAbonnementRepository, AbonnementRepository>();
        services.AddScoped<ISoldAbonnementRepository, SoldAbonnementRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddDbContext<DbServiceContext>(options => { options.UseNpgsql(dbConnectionString); });
    }
}