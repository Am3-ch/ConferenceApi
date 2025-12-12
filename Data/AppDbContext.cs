using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Speaker> Speakers { get; set; }
    public DbSet<Talk> Talks { get; set; }
    public DbSet<TalkRegistration> TalkRegistrations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Users configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PasswordHash).IsRequired();
            
            entity.HasMany(e => e.RefreshTokens)
                  .WithOne(e => e.User)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasOne(e => e.Speaker)
                  .WithOne(e => e.User)
                  .HasForeignKey<Speaker>(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // RefreshTokens configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Token).IsUnique();
            entity.Property(e => e.Token).IsRequired();
        });

        // Speakers configuration
        modelBuilder.Entity<Speaker>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserId).IsUnique();
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Bio).IsRequired().HasMaxLength(1000);
            
            entity.HasMany(e => e.Talks)
                  .WithOne(e => e.Speaker)
                  .HasForeignKey(e => e.SpeakerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Talks configuration
        modelBuilder.Entity<Talk>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ScheduledAt);
            entity.HasIndex(e => e.Category);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            
            entity.HasMany(e => e.Registrations)
                  .WithOne(e => e.Talk)
                  .HasForeignKey(e => e.TalkId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // TalkRegistrations configuration
        modelBuilder.Entity<TalkRegistration>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TalkId, e.UserId }).IsUnique(); // One registration per user per talk
            
            entity.HasOne(e => e.User)
                  .WithMany(e => e.TalkRegistrations)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}