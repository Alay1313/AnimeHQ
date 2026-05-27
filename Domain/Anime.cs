using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("Anime")]
public class Anime
{

    [Key]
    public int Id {get; set; }

    [Required]
    public int AnimeListId {get; set; }

    [Required, MaxLength(255)]
    public required string Title {get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public required string ImageURL {get; set; }

    [Range(0, 10)]
    public float Score { get; set; }
    public required string Synopsis { get; set; }

    [MaxLength(50)]
    public required string Type { get; set; }
    public int Episodes { get; set;}

    [MaxLength(50)]
    public required string Status { get; set;}
    public DateTime AiredFrom { get; set; }
    public DateTime AiredTo {get; set; }
    public DateTime CachedAt {get; set; } = DateTime.UtcNow;

    public ICollection<Episode> EpisodesList { get; set; } = [];
    public ICollection<Review> Reviews { get; set;} = [];
    public ICollection<Favorite> FavoriteBy {get; set; } = [];
    public ICollection<AnimeGenre> AnimeGenres { get; set;} = [];
    
  





}
