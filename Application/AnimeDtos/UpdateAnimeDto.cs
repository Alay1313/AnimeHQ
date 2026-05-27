using System;

namespace Application;

public class UpdateAnimeDto
{
    public int AnimeListId { get; set;}
    public string? Title { get; set;} 
    public string? ImageURL { get; set;}
    public float Score { get; set;}
    public string? Synopsis { get; set;} 
    public string? Type { get; set;}
    public int Episodes { get; set;}
    public required string Status { get; set;}
    public DateTime AiredFrom { get; set;}
    public DateTime AiredTo { get; set;}
    public DateTime CachedAt { get; set; }

    public IEnumerable<int>?GenreIds { get; set;}

}
