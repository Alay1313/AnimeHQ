using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;

[Table("AnimeGenres")]
public class AnimeGenre
{   
    [Key] // Marks this as part of the key (the other part is configured in DbContext)
    [Column(Order = 1)]
    public int AnimeId { get; set; } 

    [Key] 
    [Column(Order = 2)]
    public int GenreId { get; set; }

    
    public Anime? Anime { get; set; } 
    public Genre? Genre { get; set; }
}
