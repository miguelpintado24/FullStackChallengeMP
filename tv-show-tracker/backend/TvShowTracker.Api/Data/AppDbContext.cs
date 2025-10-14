using Microsoft.EntityFrameworkCore;
using TvShowTracker.Api.Models;

namespace TvShowTracker.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<User> Users => Set<User>();
        public DbSet<TvShow> TvShows => Set<TvShow>();
        public DbSet<Episode> Episodes => Set<Episode>();
        public DbSet<Actor> Actors => Set<Actor>();
        public DbSet<Favorite> Favorites => Set<Favorite>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserId, f.TvShowId });

            modelBuilder.Entity<TvShow>()
                .HasMany(s => s.Episodes)
                .WithOne(e => e.TvShow)
                .HasForeignKey(e => e.TvShowId);

            modelBuilder.Entity<TvShow>()
                .HasMany(s => s.Actors)
                .WithMany(a => a.TvShows);
        }
    }
}
