using System;
using System.ComponentModel.DataAnnotations;

namespace Application;

public class CreateReviewDto
{   
    [Required]public string? UserId { get; set; }
    [Required]public int AnimeId { get; set; }
    [Required, MaxLength(2000)]public string? Content { get; set; }
    [Range(0.1f, 10f)]public float Rating { get; set; }
    
}