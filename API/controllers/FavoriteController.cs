using Microsoft.AspNetCore.Mvc;
using Application;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Claims; 

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoriteController : ControllerBase
{
    private readonly IFavoriteService _favoriteService;

    
    public FavoriteController(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    //Adds an anime to the user's favorites.
    
    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] CreateFavoriteDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Security Note: In a production app, extract the UserId from the 
        // authenticated user's JWT token (e.g., User.FindFirstValue(ClaimTypes.NameIdentifier)) 
        // instead of trusting the request body to prevent users from spoofing others' accounts.
        
        var result = await _favoriteService.AddFavoriteAsync(dto, ct);
        
        if (result == null)
        {
            return Conflict("This anime is already in your favorites.");
        }

        // Return 201 Created with a link to the user's favorites list
        return CreatedAtAction(nameof(GetUserFavorites), new { userId = dto.UserId }, result);
    }

    
    //Gets all favorites for a specific user.
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserFavorites(string userId, CancellationToken ct = default)
    {
        var favorites = await _favoriteService.GetUserFavoritesAsync(userId, ct);
        return Ok(favorites);
    }


    //Checks if a specific anime is in the user's favorites.

    [HttpGet("check")]
    public async Task<IActionResult> IsFavorite([FromQuery] string userId, [FromQuery] int animeId, CancellationToken ct = default)
    {
        var isFavorite = await _favoriteService.IsFavoriteAsync(userId, animeId, ct);
        return Ok(new { isFavorite });
    }

    
    // Removes an anime from the user's favorites.

    [HttpDelete]
    public async Task<IActionResult> RemoveFavorite([FromQuery] string userId, [FromQuery] int animeId, CancellationToken ct = default)
    {
        var success = await _favoriteService.RemoveFavoriteAsync(userId, animeId, ct);
        
        if (!success)
        {
            return NotFound("Favorite not found or already removed.");
        }

        
        return NoContent(); 
    }
}
    

