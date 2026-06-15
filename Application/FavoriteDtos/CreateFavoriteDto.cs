using System;
using System.ComponentModel.DataAnnotations;

namespace Application;

public class CreateFavoriteDto 
{ 
    [Required] public string UserId { get; set; } = string.Empty; 
    [Required] public int AnimeId { get; set; } 
    
    
}
