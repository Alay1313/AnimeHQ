using System;
using Application;
using Application.AnimeDtos;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Persistence;


namespace API.controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimeController : ControllerBase
{

     private readonly AnimeService _animeService;

    public AnimeController(AnimeService animeService)
    {
        _animeService = animeService;
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var anime = await _animeService.GetAnimeByIdAsync(id);
        if (anime == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(anime);
        }
    }


    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
         try
    {
        var results = await _animeService.SearchAnimesAsync(query, page, pageSize);
        return Ok(results);
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR:");
        Console.WriteLine(ex.ToString());

        return StatusCode(500, ex.ToString());
    }
    }


    [HttpGet("jikan/{malId}")]
    public async Task<IActionResult> FetchAndCacheFromJikan(int malId)
    {
        var animeDto = await _animeService.FetchAndCacheFromJikanAsync(malId);

        if (animeDto == null)
        {
            return NotFound($"Anime with MAL ID {malId} not found on Jikan API.");
        }
        else
        {
            return CreatedAtAction(nameof(GetById), new { id = animeDto.AnimeListId }, animeDto);
        }
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AnimeDto dto)
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
public async Task<IActionResult> GetTopAiringAnimes()
{
    // Assuming your repo/service supports filtering by status or you sort by score/date
    var animes = await _animeService.SearchAnimesAsync("Airing", page: 1, pageSize: 50);
    return Ok(animes);
}
}