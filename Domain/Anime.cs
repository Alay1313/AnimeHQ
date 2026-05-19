using System;

namespace Domain;

public class Anime
{

    public int Id {get; set; }
    public int AnimeListId {get; set; }
    public required string Title {get; set; } = string.Empty;
    public string? ImageURL {get; set; } //? allows for nullable 
    public DateTime CachedAt {get; set; }
    public List<Favorite> Favorites {get; set; } = new();





}
