using DataAccess.Relational.Abonnement.Entities;
using DataAccess.Relational.Auth.Entities;
using DataAccess.Relational.Event.Entities;
using DataAccess.Relational.Lesson.Entities;
using DataAccess.Relational.Protocol.Entities;
using DataAccess.Relational.Relations;
using DataAccess.Relational.Room.Entities;
using Microsoft.EntityFrameworkCore;
using Model.Protocol;

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
    public DbSet<AbonnementsLessonsEntity> AbonnementsLessons => Set<AbonnementsLessonsEntity>();
    public DbSet<SoldAbonnementEntity> SoldAbonnements => Set<SoldAbonnementEntity>();
    public DbSet<EventEntry> Events => Set<EventEntry>();

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
        
        builder.Entity<LessonEntity>()
            .HasOne(pt => pt.Trainer)
            .WithMany()
            .HasForeignKey(pt => pt.TrainerId);
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
    
    private void OnModelCreatingEvent(ModelBuilder builder)
    {
        builder.Entity<EventEntry>()
            .HasOne(pt => pt.Lesson)
            .WithMany()
            .HasForeignKey(pt => pt.LessonId);
        
        builder.Entity<EventEntry>()
            .HasOne(pt => pt.Room)
            .WithMany()
            .HasForeignKey(pt => pt.RoomId);
    }

    private void OnModelCreatingProtocol(ModelBuilder builder)
    {
        builder.Entity<ProtocolEntry>()
            .HasMany(b => b.Clients)
            .WithMany(b => b.Protocols)
            .UsingEntity<ProtocolClient>(
                j => j
                    .HasOne<PersonEntity>()
                    .WithMany()
                    .HasForeignKey(pt => pt.PersonId),
                j => j
                    .HasOne<ProtocolEntry>()
                    .WithMany()
                    .HasForeignKey(pt => pt.ProtocolId));
    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        OnModelCreatingAuth(builder);
        OnModelCreatingRoom(builder);
        OnModelCreatingLesson(builder);
        OnModelCreatingAbonnement(builder);
        OnModelCreatingEvent(builder);
        OnModelCreatingProtocol(builder);
    }
}