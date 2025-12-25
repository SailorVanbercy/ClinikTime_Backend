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
}