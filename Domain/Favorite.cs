using System;

namespace Domain;

public class Favorite
{
    public int Id { get; set; }
    public string UserId { get; set;} = "guest";
    public int AnimeId { get; set; }
    public Anime? Anime { get; set; } 

    
}
