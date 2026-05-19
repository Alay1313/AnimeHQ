using System;

namespace Domain;

public class Review
{
    public int Id { get; set; }
    public string UserId { get; set; } = Guid.NewGuid().ToString();
    public required string AnimeId { get; set; }
    public int Rating { get; set; }
    public required string Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public required User User { get; set;}
    public required Anime Anime { get; set; }
   

}
