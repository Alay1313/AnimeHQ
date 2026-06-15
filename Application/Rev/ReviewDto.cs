using System;
using System.ComponentModel.DataAnnotations;

namespace Application;



    public class ReviewDto
    {
    
    public int ReviewId { get; set; }
    public string? UserId { get; set; }
    public int AnimeId { get; set; }
    public required string Content { get; set; }
    public float Rating { get; set; }
    public DateTime CreatedAt { get; set; }

    // Populated via AutoMapper from Navigation Properties
    public string? UserName { get; set; }
    public string? AnimeTitle { get; set; }
        
};






