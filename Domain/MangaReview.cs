using System.ComponentModel.DataAnnotations;

namespace Domain;

public class MangaReview
{   
    [Key]
    public int ReviewId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int MangaId { get; set; }
    public string Content { get; set; } = string.Empty;
    public float Rating { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Manga? Manga { get; set; }
}