using System;

namespace Application;

public class EpisodeDto 
{ 
    public int Id { get; set; } 
    public int AnimeId { get; set; } 
    public required string Title { get; set; } 
    public string Synopsis { get; set; } 
    public DateTime AirDate { get; set; } 
    public required string Duration { get; set; } 
    
}
