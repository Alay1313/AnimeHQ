using System;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
namespace Persistence;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Anime> Animes => Set<Anime>();
    public DbSet<Favorite> Favorites => Set<Favorite>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anime>().HasIndex(a => a.AnimeListId).IsUnique();
        modelBuilder.Entity<Favorite>().HasIndex(f => new{ f.UserId, f.AnimeId }).IsUnique();
    }

}
