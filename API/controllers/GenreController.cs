using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application;
using AutoMapper;
using Domain;
namespace API.controllers;

[ApiController]
[Route("api/[controller]")]
public class GenreController : ControllerBase
{
    private readonly IGenreRepo _repo;
    private readonly IMapper _mapper;
    public GenreController(IGenreRepo repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
         var genre = await _repo.GetByIdAsync(id);
         return genre == null ? NotFound() : Ok(_mapper.Map<GenreDto>(genre));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAllAsync());
    } 

  [HttpPost]
    public async Task<IActionResult> Create([FromBody] GenreDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.Name)) return BadRequest();
        var genre = _mapper.Map<Genre>(dto);
        var created = await _repo.CreateAsync(genre);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<GenreDto>(created));
    }

 

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
      
        var success = await _repo.DeleteAsync(id);
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


