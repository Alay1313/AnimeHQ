using System;

namespace Application;

public class CreateFavoriteDto 
{ 
    public string UserId { get; set; } = "guest"; 
    public int AnimeId { get; set; } 
    public int GenreId { get; set; } 
    
    
}
