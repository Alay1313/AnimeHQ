using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("AnimeGenres")]
public class AnimeGenre
{   
    
    public int AnimeId { get; set; } 

    
    public int GenreId { get; set; }


    public Anime Anime { get; set; } 
    public Genre Genre { get; set; }
    

}
