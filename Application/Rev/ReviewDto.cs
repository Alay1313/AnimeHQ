using System;
using System.ComponentModel.DataAnnotations;

namespace Application;



    public class ReviewDto
    {
    
    public int Id { get; set; }
    public string? UserId { get; set;}
    public int AnimeId { get; set;}
    public int ReviewId { get; set;}
    public required string Content { get; set; }
    public float Rating { get; set;}
    public required string Comment { get; set;}
    public required DateTime CreatedAt { get; set;}

    public string? UserName { get; set; }
    public string? AnimeTitle { get; set; }
        
};






