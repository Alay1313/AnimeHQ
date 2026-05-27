using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Application.UserDtos;

using Domain;
using Application;
namespace API.controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{

    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await _userService.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        else
        {
            return Ok(user);
        }
    }



    [HttpGet("username/{username}")]
    public async Task<IActionResult> GetByUsername(string username)
    {
        var user = await _userService.GetByUsernameAsync(username);
        return user == null ? NotFound() : Ok(user);
    }



    [HttpPost]
    public async Task<IActionResult> Create([FromBody] UserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _userService.CreateAsync(dto);
        if (created == null) return BadRequest("Failed to create user.");

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }



    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UserDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Ensure route ID matches DTO payload ID
        if (id != dto.Id) return BadRequest("Route ID and payload ID must match.");

        var updated = await _userService.UpdateAsync(id, dto);
        return updated == null ? NotFound() : Ok(updated);
    }






}




    

