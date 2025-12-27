using System.Security.Claims;
using ClinikTime.service.Disponibilite;
using ClinikTime.service.Medecin;
using Infrastructure.user.Dto.Disponibilite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.controllers.Disponibilite;
[ApiController]
[Route("api/v1/disponibilite")]
public class DisponibiliteMedecinController(DisponibiliteMedecinService service, MedecinService medecinService) : ControllerBase
{
    [Authorize(Roles = "Medecin")]
    [HttpPost("ouvrir")]
    public async Task<ActionResult> Ouvrir(BloquerDisponibiliteDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var medecin = await medecinService.GetByUtilisateurIdAsync(userId);
        if (medecin == null)
            return Unauthorized();
        await service.OuvrirAsync(medecin.Id, dto);
        return Ok("Disponibilité créée");
    }

    [Authorize(Roles = "Medecin")]
    [HttpPost("bloquer")]
    public async Task<ActionResult> Bloquer(BloquerDisponibiliteDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var medecin = await medecinService.GetByUtilisateurIdAsync(userId);
        if (medecin == null)
            return Unauthorized();
        await service.BloquerAsync(medecin.Id, dto);
        return Ok("Disponibilité bloquée");
    }

    [Authorize(Roles = "Medecin")]
    [HttpGet("me")]
    public async Task<ActionResult<List<DisponibiliteDto>>> GetMyDispo()
    {
        var id = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        return await service.GetMyDispo(id);
    }
    
}