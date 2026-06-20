using System;
using Application;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Persistence;


namespace API.controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimeController : ControllerBase
{

     private readonly IAnimeService _animeService;

    public AnimeController(IAnimeService animeService)
    {
        _animeService = animeService;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        try
        {
            var anime = await _animeService.GetByIdAsync(id, ct);
            if (anime == null) return NotFound();
            return Ok(anime);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            return StatusCode(500, new { message = ex.Message, innerError = ex.InnerException?.Message });
        }
    }


  [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        try
        {
            var results = await _animeService.SearchAsync(query, page, pageSize);
            return Ok(results);
        }
        catch (Exception ex)
        {
            Console.WriteLine("ERROR:");
            Console.WriteLine(ex.ToString());

            return StatusCode(500, ex.ToString());
        }
    }


    [HttpPost("{malId}/sync")]
    public async Task<IActionResult> FetchAndCacheFromJikan(int malId, CancellationToken ct)
    {
       var result = await _animeService.SyncFromJikanAsync(malId, ct); 
    return Ok(result);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAnimeDto dto)
    {
        if (dto == null || dto.AnimeListId != id)
            return BadRequest("Invalid update payload or ID mismatch.");


        var updated = await _animeService.UpdateAsync(id, dto);

        return updated == null ? NotFound() : Ok(updated);
    }


    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _animeService.DeleteAsync(id);
        return success ? NoContent() : NotFound();
    }


[HttpGet("top-airing")]
public async Task<IActionResult> GetTopAiring([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? type = null, CancellationToken ct = default)
{
     try 
        {
            var result = await _animeService.GetTopAiringAsync(page, pageSize, type);
            return Ok(result);
        }
        catch (Exception ex)
        {
            //This catches the crash and sends the exact error message to the frontend!
            return StatusCode(500, new { 
                message = ex.Message, 
                innerError = ex.InnerException?.Message 
            });
        }
}



[HttpGet("popular")]
public async Task<IActionResult> GetPopular([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? type = null, CancellationToken ct = default)
{
    var result = await _animeService.GetPopularAsync(page, pageSize, type, ct);
    return Ok(result);
}



[HttpGet("seasonal")]
public async Task<IActionResult> GetSeasonal([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? type = null, CancellationToken ct = default)
{
    var result = await _animeService.GetSeasonalAsync(page, pageSize, type, ct);
    return Ok(result);
}





// Gets upcoming anime from the Jikan API, optionally filtered by type (tv, movie, ova, ona, special, music, or all).
[HttpGet("upcoming")]
public async Task<IActionResult> GetUpcoming([FromQuery] int page = 1, [FromQuery] int pageSize = 25, [FromQuery] string? type = null, CancellationToken ct = default)
{
    var result = await _animeService.GetUpcomingAsync(page, pageSize, type, ct);
    return Ok(result);
}


}