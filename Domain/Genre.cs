using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain;


[Table("Genres")]
public class Genre
{
    [Key]
    public int Id { get; set; }


    [Required, MaxLength(100)]
    public required string Name {get; set; }
    

    public ICollection<AnimeGenre> AnimeGenres { get; set; } = new List<AnimeGenre>();
}
