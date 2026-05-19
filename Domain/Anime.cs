using System;

namespace Domain;

public class Anime
{

    public int Id {get; set; }
    public int AnimeListId {get; set; }
    public required string Title {get; set; } = string.Empty;
    public required string ImageURL {get; set; } //? allows for nullable 
    public decimal Score { get; set; }
    public required string Synopsis { get; set; }
    public required string Type { get; set; }
    public int Episodes { get; set;}
    public required string Status { get; set;}
    public DateTime AiredFrom { get; set; }
    public DateTime AiredTo {get; set; }
    public DateTime CachedAt {get; set; } = DateTime.UtcNow;

    public ICollection<Episode> EpisodesList { get; set; } = new List<Episode>();
    public ICollection<Review> Reviews { get; set;} = new List<Review>();
    public ICollection<Favorite> FavoriteBy {get; set; } = new List<Favorite>();
    public ICollection<AnimeGenre> AnimeGenres { get; set;} = new List<AnimeGenre>();
    
  





}
