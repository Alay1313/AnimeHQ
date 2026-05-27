using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace Domain;

[Table("Users")]
public class User
{
    [Key, MaxLength(450)]
    public string Id { get; set; }  = Guid.NewGuid().ToString();

    [Required, MaxLength(100)]
    public required string UserName { get; set; }

    [Required, MaxLength(250)]
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();

}
