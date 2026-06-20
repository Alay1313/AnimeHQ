using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("Manga")]
public class Manga
{
    [Key]
    public int MangaId { get; set; } // MAL ID as PK

    [Required, MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string ImageURL { get; set; } = string.Empty;

    public float Score { get; set; }
    public string Synopsis { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Type { get; set; } = string.Empty;

    public int Chapters { get; set; }
    public int Volumes { get; set; }

    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;

    public DateTime PublishedFrom { get; set; }
    public DateTime PublishedTo { get; set; }
    public DateTime CachedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MangaReview> Reviews { get; set; } = [];
    public ICollection<MangaFavorite> Favorites { get; set; } = [];
}