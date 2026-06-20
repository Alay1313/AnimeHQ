using System.ComponentModel.DataAnnotations;

namespace Domain;

public class MangaFavorite
{
    [Key]
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int MangaId { get; set; }
    public DateTime CreatedAt { get; set; }

    public User? User { get; set; }
    public Manga? Manga { get; set; }
}