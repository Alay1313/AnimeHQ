using Microsoft.AspNetCore.Mvc;
using Application;
using System.Threading;
using System.Threading.Tasks;

namespace API.Controllers; 

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;

    
    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

   
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct = default)
    {
        var review = await _reviewService.GetByIdAsync(id);
        return review == null ? NotFound($"Review with ID {id} not found.") : Ok(review);
    }

    
    [HttpGet("anime/{animeId}")]
    public async Task<IActionResult> GetByAnimeId(int animeId, CancellationToken ct = default)
    {
        var reviews = await _reviewService.GetByAnimeIdAsync(animeId);
        return Ok(reviews);
    }

   
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetByUserId(string userId, CancellationToken ct = default)
    {
        var reviews = await _reviewService.GetByUserIdAsync(userId);
        return Ok(reviews);
    }

    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReviewDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        var created = await _reviewService.CreateAsync(dto, ct);
        
        // Return 201 Created with a link to the new resource
        return CreatedAtAction(nameof(GetById), new { id = created.ReviewId }, created);
    }

    
    //Updates an existing review (only Content and Rating).
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateReviewDto dto, CancellationToken ct = default)
    {
        if (!ModelState.IsValid) 
            return BadRequest(ModelState);

        var updated = await _reviewService.UpdateAsync(id, dto, ct);
        
        return updated == null 
            ? NotFound($"Review with ID {id} not found.") 
            : Ok(updated);
    }

    
    //Deletes a review by its ID
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct = default)
    {
        var success = await _reviewService.DeleteAsync(id, ct);
        
        return success 
            ? NoContent() // 204 No Content is standard for successful deletes
            : NotFound($"Review with ID {id} not found.");
    }
}
