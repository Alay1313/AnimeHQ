namespace Application;

public class CreateMangaDto
{
    public int MangaId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageURL { get; set; } = string.Empty;
    public float Score { get; set; }
    public string Synopsis { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int Chapters { get; set; }
    public int Volumes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime PublishedFrom { get; set; }
    public DateTime PublishedTo { get; set; }
}