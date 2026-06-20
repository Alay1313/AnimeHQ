using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;


public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Anime> Animes { get; set; }
    public DbSet<Episode> Episodes { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Favorite> Favorites { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<AnimeGenre> AnimeGenres { get; set; }
    public DbSet<Manga> Mangas { get; set; }
    public DbSet<MangaFavorite> MangaFavorites { get; set; }
    public DbSet<MangaReview> MangaReviews { get; set; }

   protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // 1. Genre Configurations
    modelBuilder.Entity<Genre>().HasIndex(g => g.Name).IsUnique();
    
    modelBuilder.Entity<Genre>().HasData(
        new Genre { Id = 1, Name = "Action" },
        new Genre { Id = 2, Name = "Adventure" },
        new Genre { Id = 3, Name = "Comedy" },
        new Genre { Id = 4, Name = "Drama" },
        new Genre { Id = 5, Name = "Fantasy" },
        new Genre { Id = 6, Name = "Mystery" },
        new Genre { Id = 7, Name = "Horror" },
        new Genre { Id = 8, Name = "Thriller" },
        new Genre { Id = 9, Name = "Romance" },
        new Genre { Id = 10, Name = "Supernatural" }
    );

    // 2. AnimeGenre (Join Table) Configurations
    modelBuilder.Entity<AnimeGenre>(entity =>
    {
        entity.HasKey(e => new { e.AnimeId, e.GenreId });

        entity.HasOne(ag => ag.Anime)
              .WithMany(a => a.AnimeGenres)
              .HasForeignKey(ag => ag.AnimeId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(ag => ag.Genre)
              .WithMany(g => g.AnimeGenres)
              .HasForeignKey(ag => ag.GenreId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // 3. Episode Configurations
    modelBuilder.Entity<Episode>(entity =>
    {
        entity.HasOne(e => e.Anime)
              .WithMany(a => a.EpisodesList)
              .HasForeignKey(e => e.AnimeId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // 4. Review Configurations
    modelBuilder.Entity<Review>(entity =>
    {
        entity.HasOne(r => r.User)
              .WithMany(u => u.Reviews)
              .HasForeignKey(r => r.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(r => r.Anime)
              .WithMany(a => a.Reviews)
              .HasForeignKey(r => r.AnimeId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    // 5. Favorite Configurations
    modelBuilder.Entity<Favorite>(entity =>
    {
        entity.HasOne(f => f.User)
              .WithMany(u => u.Favorites)
              .HasForeignKey(f => f.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(f => f.Anime)
              .WithMany(a => a.FavoriteBy)
              .HasForeignKey(f => f.AnimeId)
              .OnDelete(DeleteBehavior.Cascade);
    });



    modelBuilder.Entity<MangaFavorite>(entity =>
    {
        entity.HasOne(f => f.User)
              .WithMany()
              .HasForeignKey(f => f.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(f => f.Manga)
              .WithMany(m => m.Favorites)
              .HasForeignKey(f => f.MangaId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    modelBuilder.Entity<MangaReview>(entity =>
    {
        entity.HasOne(r => r.User)
              .WithMany()
              .HasForeignKey(r => r.UserId)
              .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(r => r.Manga)
              .WithMany(m => m.Reviews)
              .HasForeignKey(r => r.MangaId)
              .OnDelete(DeleteBehavior.Cascade);
    });


      modelBuilder.Entity<Anime>()
            .Property(a => a.AnimeListId)
            .ValueGeneratedNever();

      modelBuilder.Entity<Manga>()
            .Property(m => m.MangaId)
            .ValueGeneratedNever();
}
}
