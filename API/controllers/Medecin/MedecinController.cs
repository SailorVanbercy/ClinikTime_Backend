using System.Security.Claims;
using ClinikTime.service.Medecin;
using Infrastructure.user.Dto;
using Infrastructure.user.Dto.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.controllers.Medecin;
[ApiController]
[Route("api/v1/medecin")]
public class MedecinController(MedecinService service) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<MedecinDto>> CreateMedecin([FromBody]CreateMedecinDto medecin)
    {
        return await service.CreateMedecin(medecin);
        
    }

    [Authorize(Roles = "Medecin")]
    [HttpGet("me")]
    public async Task<ActionResult<MedecinDto>> GetMyMedecin()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var medecin = await service.GetMyMedecin(userId);
        return Ok(medecin);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<List<MedecinDto>>> GetAllMedecins([FromQuery] int? specialiteId)
    {
        var medecins = await service.GetAllAsync(specialiteId);
        return Ok(medecins);
    }
}