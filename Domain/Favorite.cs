using System;

namespace Domain;

public class Favorite
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int AnimeId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Anime? Anime { get; set; }
}