using System;

namespace Application;

public class FavoriteDto 
{ 
    public int Id { get; set; } 
    public string? UserId { get; set; } 
    public int AnimeId { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Populated via AutoMapper from Navigation Properties
    public string? UserName { get; set; }
    public string? AnimeTitle { get; set; } 
}
