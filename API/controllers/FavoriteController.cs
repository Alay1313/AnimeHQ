using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application;
using AutoMapper;
using Domain;

namespace API.controllers;

[ApiController]
[Route("api/[controller]")]
public class FavoriteController : ControllerBase
{
   private readonly FavoriteService _favoriteService;
    
    public FavoriteController(FavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }



    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetUserFavorites(string userId)
    {
        var favorites = await _favoriteService.GetUserFavoritesAsync(userId);
        return Ok(favorites);
    }


    [HttpGet("check")]
    public async Task<IActionResult> IsFavorite([FromQuery] string userId, [FromQuery] int animeId)
    {
        var isFav = await _favoriteService.IsFavoriteAsync(userId, animeId);
        return Ok(new { IsFavorite = isFav });
    }


    [HttpPost]
    public async Task<IActionResult> AddFavorite([FromBody] CreateFavoriteDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var result = await _favoriteService.AddFavoriteAsync(dto);
        if (result == null)
            return Conflict("This anime is already in your favorites.");

        
        return CreatedAtAction(nameof(GetUserFavorites), new { userId = result.UserId }, result);
    }



    [HttpDelete]
    public async Task<IActionResult> RemoveFavorite([FromQuery] string userId, [FromQuery] int animeId)
    {
        var success = await _favoriteService.RemoveFavoriteAsync(userId, animeId);
        return success ? NoContent() : NotFound("Favorite relationship not found.");
    }

    

  

  
}
    

