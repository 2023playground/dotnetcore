using dotnet;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options)
        : base(options) { }

    public DbSet<Film> Films { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<FilmSubscription> FilmSubscription { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(e => e.FilmList)
            .WithMany(e => e.UserList)
            .UsingEntity<FilmSubscription>();

        modelBuilder.Entity<FilmSubscription>()
            .HasIndex(fs => new { fs.UserId, fs.FilmId })
            .IsUnique();
    }
}
