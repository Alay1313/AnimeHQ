using Application;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MangaController : ControllerBase
{
    private readonly IMangaService _mangaService;

    public MangaController(IMangaService mangaService)
    {
        _mangaService = mangaService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        try
        {
            var manga = await _mangaService.GetByIdAsync(id, ct);
            return Ok(manga);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1, int pageSize = 25, CancellationToken ct = default)
    {
        var results = await _mangaService.SearchAsync(query, page, pageSize, ct);
        return Ok(results);
    }

    [HttpGet("top")]
    public async Task<IActionResult> GetTop([FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default)
    {
        var results = await _mangaService.GetTopMangaAsync(page, pageSize, ct);
        return Ok(results);
    }

    [HttpPost("{id}/sync")]
    public async Task<IActionResult> Sync(int id, CancellationToken ct)
    {
        try
        {
            var result = await _mangaService.SyncFromJikanAsync(id, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpPost("favorites")]
    public async Task<IActionResult> AddFavorite([FromBody] CreateMangaFavoriteDto dto, CancellationToken ct)
    {
        var result = await _mangaService.AddFavoriteAsync(dto, ct);
        if (result == null) return Conflict("Already in favorites.");
        return Ok(result);
    }

    [HttpDelete("favorites")]
    public async Task<IActionResult> RemoveFavorite([FromQuery] string userId, [FromQuery] int mangaId, CancellationToken ct)
    {
        var success = await _mangaService.RemoveFavoriteAsync(userId, mangaId, ct);
        return success ? NoContent() : NotFound();
    }

    [HttpGet("favorites/check")]
    public async Task<IActionResult> IsFavorite([FromQuery] string userId, [FromQuery] int mangaId, CancellationToken ct)
    {
        var isFavorite = await _mangaService.IsFavoriteAsync(userId, mangaId, ct);
        return Ok(new { isFavorite });
    }

    [HttpPost("reviews")]
    public async Task<IActionResult> CreateReview([FromBody] CreateMangaReviewDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var result = await _mangaService.CreateReviewAsync(dto, ct);
        return Ok(result);
    }

    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetReviews(int id, CancellationToken ct)
    {
        var reviews = await _mangaService.GetReviewsAsync(id, ct);
        return Ok(reviews);
    }

    [HttpDelete("reviews/{reviewId}")]
    public async Task<IActionResult> DeleteReview(int reviewId, CancellationToken ct)
    {
        var success = await _mangaService.DeleteReviewAsync(reviewId, ct);
        return success ? NoContent() : NotFound();
    }


    [HttpGet("favorites/user/{userId}")]
    public async Task<IActionResult> GetUserFavorites(string userId, CancellationToken ct)
    {
        var favorites = await _mangaService.GetUserFavoritesAsync(userId, ct);
        return Ok(favorites);
    }

    [HttpGet("reviews/user/{userId}")]
    public async Task<IActionResult> GetUserReviews(string userId, CancellationToken ct)
    {
        var reviews = await _mangaService.GetUserReviewsAsync(userId, ct);
        return Ok(reviews);
    }


}