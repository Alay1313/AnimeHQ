using System.ComponentModel.DataAnnotations;

namespace Application;

public class CreateMangaFavoriteDto
{
    [Required] public string UserId { get; set; } = string.Empty;
    [Required] public int MangaId { get; set; }
}