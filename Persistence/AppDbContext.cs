using System;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Persistence;

public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    
    public  DbSet<User> Users {get;set;}
    public  DbSet<Anime> Animes {get;set;}
    public  DbSet<Episode> Episodes {get;set;}
    public  DbSet<Review> Reviews {get;set;}
    public  DbSet<Favorite> Favorites {get;set;}
    public  DbSet<Genre> Genres {get; set;}
    public  DbSet<AnimeGenre> AnimeGenres {get;set;}
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Anime>().HasIndex(a => a.AnimeListId).IsUnique();


         modelBuilder.Entity<Genre>()
         .HasIndex(g => g.Name)
         .IsUnique();


         modelBuilder.Entity<AnimeGenre>(entity =>
         {
             entity.HasKey(e => new { e.AnimeId, e.GenreId});
         });
        

        
         modelBuilder.Entity<Review>()
         .HasOne(r => r.User)
         .WithMany(u => u.Reviews)
         .HasForeignKey(r => r.UserId);


         modelBuilder.Entity<Favorite>()
         .HasOne(f => f.Anime)
         .WithMany(u => u.FavoriteBy)
         .HasForeignKey(f => f.AnimeId);



         modelBuilder.Entity<AnimeGenre>()
         .HasOne(g => g.Anime)
         .WithMany(ag => ag.AnimeGenres)
         .HasForeignKey(g => g.AnimeId);



         modelBuilder.Entity<Genre>().HasData(

            new Genre { Id = 1, Name = "Action"},
            new Genre { Id = 2, Name = "Adventure"},
            new Genre { Id = 3, Name = "Comedy"},
            new Genre { Id = 4, Name = "Drama"},
            new Genre { Id = 5, Name = "Fantasy"},
            new Genre { Id = 6, Name = "Mystery"},
            new Genre { Id = 7, Name = "Horror"},
            new Genre { Id = 8, Name = "Thriller"},
            new Genre { Id = 9, Name = "Romance"},
            new Genre { Id = 10, Name = "Supernatural"}


         );

       

       
    }

}
