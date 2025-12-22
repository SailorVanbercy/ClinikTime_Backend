using ClinikTime.service;
using Domain.models;
using Infrastructure.user.Dto;
using Infrastructure.user.EF;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.controllers.user;

[ApiController]
[Route("api/v1/user")]
public class UserController(UserService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await service.GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await service.GetByIdAsync(id);
        if(user == null)
            return  NotFound();
        return Ok(user);
    }

    [HttpGet("by-email")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserDto>> GetByEmail([FromQuery]string email)
    {
        var user = await service.GetByEmailAsync(email);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }
    

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<UserDto>> Save(CreateUserDto dto)
    {
        var user = await service.AddAsync(dto);
        return CreatedAtAction(
            nameof(GetByEmail),
            new { email = user.Email },
            user
        );
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Update(int id, UpdateUserDto dto)
    {
        var updatedUser = await service.UpdateAsync(id, dto);

        if (!updatedUser)
            return NotFound();
        return NoContent();
    }
    
}