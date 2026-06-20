namespace Application;

public class MangaFavoriteDto
{
    public int Id { get; set; }
    public string? UserId { get; set; }
    public int MangaId { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserName { get; set; }
    public string? MangaTitle { get; set; }
}