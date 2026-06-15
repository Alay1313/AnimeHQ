using System;

namespace Application;

public class UserDto 
{ 
    public string Id { get; set; } = string.Empty;
    public required string Username { get; set; } 
    public required string Email { get; set; } 
    public DateTime JoinedAt { get; set; }
    public int ReviewCount { get; set; }
    public int FavoriteCount { get; set; }
 
}
