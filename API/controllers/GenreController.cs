using Microsoft.AspNetCore.Mvc;
using Application;


namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private readonly IGenreService _genreService;

    public GenreController(IGenreService genreService)
    {
        _genreService = genreService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var genre = await _genreService.GetByIdAsync(id, ct);
        return genre == null ? NotFound() : Ok(genre);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var genres = await _genreService.GetAllAsync(ct);
        return Ok(genres);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGenreDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _genreService.CreateAsync(dto, ct);
        
        if (created == null)
        {
            return Conflict("A genre with this name already exists.");
        }

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var success = await _genreService.DeleteAsync(id, ct);
        return success ? NoContent() : NotFound();
    }
}


