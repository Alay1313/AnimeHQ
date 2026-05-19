using System;

namespace Domain;

public class Genre
{
    public int Id { get; set; }
    public required string Name {get; set; }

    public ICollection<AnimeGenre> AnimeGenres { get; set; } = new List<AnimeGenre>();
}
