using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieTime.Models;

namespace MovieTime.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<MovieActor> MovieActors { get; set; }
        public DbSet<Genre>Genres { get; set; }
        public DbSet<Watchlist> Watchlists { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            });

            modelBuilder.Entity<MovieActor>()
                .HasKey(fa => new { fa.MovieId, fa.ActorId });

            modelBuilder.Entity<MovieActor>()
                .HasOne(fa => fa.Movie)
                .WithMany(f => f.MovieActor)
                .HasForeignKey(fa => fa.MovieId);

            modelBuilder.Entity<MovieActor>()
                .HasOne(fa => fa.Actor)
                .WithMany(a => a.MovieActor)
                .HasForeignKey(fa => fa.ActorId);
        }
    }
}
