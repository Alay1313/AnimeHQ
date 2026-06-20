namespace Application;

public class MangaReviewDto
{
    public int ReviewId { get; set; }
    public string? UserId { get; set; }
    public int MangaId { get; set; }
    public string Content { get; set; } = string.Empty;
    public float Rating { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserName { get; set; }
    public string? MangaTitle { get; set; }
}