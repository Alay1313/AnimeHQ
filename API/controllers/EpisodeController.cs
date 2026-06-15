using Microsoft.AspNetCore.Mvc;
using Application;


namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EpisodeController : ControllerBase
{
    private readonly IEpisodeService _service;

    public EpisodeController(IEpisodeService service)
    {
        _service = service; 
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var episode = await _service.GetByIdAsync(id, ct);
        return episode == null ? NotFound() : Ok(episode);
    }

    [HttpGet("anime/{animeId}")]
    public async Task<IActionResult> GetByAnime(int animeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        var episodes = await _service.GetByAnimeIdAsync(animeId, page, pageSize, ct);
        return Ok(episodes);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEpisodeDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto, ct);
        if (created == null) return BadRequest("Failed to create episode.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEpisodeDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, dto, ct);
        return updated == null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var success = await _service.DeleteAsync(id, ct);
        return success ? NoContent() : NotFound();
    }
}





