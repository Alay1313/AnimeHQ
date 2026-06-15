using System;
using System.ComponentModel.DataAnnotations;



public class UpdateReviewDto
{
    [Required, MaxLength(2000)] public string Content { get; set; } = string.Empty;
    [Range(0.1f, 10f)] public float Rating { get; set; }
}