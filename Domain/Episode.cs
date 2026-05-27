using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

public class Episode
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; set; }
    public required int AnimeId { get; set; }
    public required string Title { get; set; }
    public required string Synopsis { get; set; }
    public DateTime AirDate { get; set;}
    public required string Duration { get; set;}
    public required string EpisodeNumber { get; set; }

    public Anime? Anime { get; set; }


}
