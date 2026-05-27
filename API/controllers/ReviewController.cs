using Application;
using AutoMapper;
using Domain;


using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Persistence;

namespace API.controllers;

[Route("api/[controller]")]
[ApiController]
public class ReviewController : ControllerBase
{
    private readonly IReviewRepo _repo;
    private readonly IMapper _mapper;
    public ReviewController(IReviewRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
        
    }

    [HttpGet("anime/{animeId}")]
    public async Task<IActionResult> GetByAnime(int animeId)
    {
        var reviews = await _repo.GetByAnimeIdAsync(animeId);
        return Ok(reviews);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto)
    {
       if (!ModelState.IsValid) return BadRequest(ModelState);

    var review = _mapper.Map<Review>(dto);
    var created = await _repo.CreateAsync(review);
    
    return CreatedAtAction(nameof(GetByAnime), new { id = created.ReviewId }, _mapper.Map<ReviewDto>(created));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _repo.DeleteAsync(id);
        if (deleted)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }
}
