using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application;


namespace API.controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodeController : ControllerBase
    {
        private readonly EpisodeService _service;
    
    public EpisodeController(EpisodeService service)
    {
            _service = service; 
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var episode = await _service.GetByIdAsync(id);
        return episode == null ? NotFound() : Ok(episode);
    }

       [HttpGet("anime/{animeId}")]
    public async Task<IActionResult> GetByAnime(int animeId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var episodes = await _service.GetByAnimeIdAsync(animeId, page, pageSize);
        return Ok(episodes);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEpisodeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _service.CreateAsync(dto);
        if (created == null) return BadRequest("Failed to create episode.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }


    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEpisodeDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _service.UpdateAsync(id, dto);
            if (updated == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(updated);
            }
        }



    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _service.DeleteAsync(id);
            if (success)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }


    }
}





