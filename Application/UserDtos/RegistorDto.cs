using System.ComponentModel.DataAnnotations;

namespace Application;


public class RegisterDto
{
    public required string Username { get; set; }

    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public required string Email { get; set; }
    public required string Password { get; set; } 
}