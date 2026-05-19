using System;

namespace Domain;

public class Episode
{
    public int Id { get; set; }
    public int AnimeId { get; set; }
    public required string Title { get; set; }
    public required string Synopsis { get; set; }
    public DateTime AirDate { get; set;}
    public required string Duration { get; set;}

    public required Anime Anime { get; set; }


}
