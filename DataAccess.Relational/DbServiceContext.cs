using DataAccess.Relational.Abonnement.Entities;
using DataAccess.Relational.Auth.Entities;
using DataAccess.Relational.Lesson.Entities;
using DataAccess.Relational.Relations;
using DataAccess.Relational.Room.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Relational;

public sealed class DbServiceContext : DbContext
{
    public DbServiceContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<AuthEntity> Auth => Set<AuthEntity>();
    public DbSet<PersonEntity> Persons => Set<PersonEntity>();
    public DbSet<ConfirmationCodeEntity> ConfirmationCodes => Set<ConfirmationCodeEntity>();
    public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
    public DbSet<RoomEntity> Rooms => Set<RoomEntity>();
    public DbSet<LessonEntity> Lessons => Set<LessonEntity>();
    public DbSet<AbonnementEntity> Abonnements => Set<AbonnementEntity>();
    public DbSet<SoldAbonnementEntity> SoldAbonnements => Set<SoldAbonnementEntity>();

    private void OnModelCreatingAuth(ModelBuilder builder)
    {
        builder.Entity<AuthEntity>()
            .HasIndex(u => u.Email)
            .IsUnique();

        builder.Entity<PersonEntity>()
            .HasOne(p => p.Auth)
            .WithOne()
            .HasForeignKey<AuthEntity>(a => a.PersonId);

        builder.Entity<ConfirmationCodeEntity>()
            .HasIndex(u => u.Token)
            .IsUnique();

        builder.Entity<ConfirmationCodeEntity>()
            .HasIndex(u => u.PersonId);

        builder.Entity<ConfirmationCodeEntity>()
            .HasOne(pt => pt.Person)
            .WithMany()
            .HasForeignKey(pt => pt.PersonId);

        builder.Entity<RefreshTokenEntity>()
            .HasIndex(u => u.PersonId)
            .IsUnique();

        builder.Entity<RefreshTokenEntity>()
            .HasOne(pt => pt.Person)
            .WithMany()
            .HasForeignKey(pt => pt.PersonId);
    }

    private void OnModelCreatingRoom(ModelBuilder builder)
    {
        builder.Entity<RoomEntity>()
            .HasIndex(u => u.Name)
            .IsUnique();
    }

    private void OnModelCreatingLesson(ModelBuilder builder)
    {
        builder.Entity<LessonEntity>()
            .HasIndex(u => u.Name)
            .IsUnique();
    }

    private void OnModelCreatingAbonnement(ModelBuilder builder)
    {
        builder.Entity<AbonnementEntity>()
            .HasIndex(u => u.Name)
            .IsUnique();

        builder.Entity<AbonnementEntity>()
            .HasMany(b => b.Lessons)
            .WithMany(b => b.Abonnements)
            .UsingEntity<AbonnementsLessonsEntity>(
                j => j
                    .HasOne<LessonEntity>()
                    .WithMany()
                    .HasForeignKey(pt => pt.LessonId),
                j => j
                    .HasOne<AbonnementEntity>()
                    .WithMany()
                    .HasForeignKey(pt => pt.AbonnementId));
        
        builder.Entity<SoldAbonnementEntity>()
            .HasMany(b => b.Lessons)
            .WithMany(b => b.SoldAbonnements)
            .UsingEntity<SoldAbonnementsLessonsEntity>(
                j => j
                    .HasOne<LessonEntity>()
                    .WithMany()
                    .HasForeignKey(pt => pt.LessonId),
                j => j
                    .HasOne<SoldAbonnementEntity>()
                    .WithMany()
                    .HasForeignKey(pt => pt.SoldAbonnementId));
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        OnModelCreatingAuth(builder);
        OnModelCreatingRoom(builder);
        OnModelCreatingLesson(builder);
        OnModelCreatingAbonnement(builder);
    }
}