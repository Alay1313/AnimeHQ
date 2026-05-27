using System;
using System.ComponentModel.DataAnnotations;

namespace Application;

public class UpdateEpisodeDto 
{ 
    [Required]public int EpisodeNumber { get; set; }
    [Required, MaxLength(255)] public required string Title { get; set; } 
    public DateTime AirDate { get; set; } 
    [MaxLength(2000)]public required string Synopsis { get; set; } 
    [MaxLength(50)]public required string Duration { get; set; } 
     
     
}
