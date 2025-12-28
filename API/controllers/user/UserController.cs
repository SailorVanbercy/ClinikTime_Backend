using System.Security.Claims;
using ClinikTime.service;
using Domain.models;
using Infrastructure.user.Dto;
using Infrastructure.user.Dto.Profile;
using Infrastructure.user.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.controllers.user;
// [Authorize] ⚠️ a remettre à la fin ici pour les test swagger
[ApiController]
[Route("api/v1/user")]
public class UserController(UserService service) : ControllerBase
{
    
    /// <summary>
    /// Retourne la liste de tous les utilisateurs (accessible seulement par un admin)
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
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
    
    [Authorize(Roles = "Admin")]
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

    /// <summary>
    /// Retourne le profil de l'utilisateur connecté (User ou Medeciin)
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    [Authorize(Roles = "User,Medecin, Admin")]
    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMyProfile()
    {
        // ID issu du jwt
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);
        var profile = await service.GetMyProfileAsync(userId);
        return Ok(profile);
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

    [Authorize(Roles = "Admin")]
    [HttpPost("{userId:int}/promote-medecin")]
    // On ajoute [FromBody] PromoteToMedecinDto dto pour recevoir les infos du formulaire
    public async Task<IActionResult> PromoteMedecin(int userId, [FromBody] PromoteToMedecinDto dto)
    {
        try 
        {
            // On envoie les 2 paramètres au service
            await service.PromoteUserToMedecin(userId, dto);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}