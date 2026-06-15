using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.Win32.SafeHandles;

namespace Domain;

public class Review
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    [Key]
    public int ReviewId { get; set; } 
    public required string UserId { get; set; }
    public required int AnimeId { get; set; } 
    public Anime? Anime { get; set; } = null!;
    public  User? User { get; set;}

    public required string Content { get; set; }
    
    public required float Rating { get; set; }     
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    

}


    
   




