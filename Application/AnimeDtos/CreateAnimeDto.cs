using System;

namespace Application;

public class CreateAnimeDto
{
    public int AnimeListId { get; set;}
    public string Title { get; set;} = string.Empty;
    public string ImageURL { get; set;} = string.Empty;
    public float Score { get; set;}
    public string Synopsis { get; set;} = string.Empty;
    public string Type { get; set;} = string.Empty;
    public int Episodes { get; set;}
    public string Status { get; set; } = string.Empty;
    public DateTime AiredFrom { get; set;}
    public DateTime AiredTo { get; set;}
    public DateTime CachedAt { get; set; }

    public IEnumerable<int>?GenreIds { get; set;} 

}
