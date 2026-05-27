using System;

namespace Domain;

public class Favorite
{
    public int Id { get; set; }
    public string UserId { get; set;} = "guest";
    public int GenreId { get; set;}
    public int AnimeId { get; set; }

    public required User User { get; set;}
    public required Anime Anime { get; set; } 

    public required Genre Genre { get; set;}

    public DateTime CreatedAt { get; set; }

   

    
}
