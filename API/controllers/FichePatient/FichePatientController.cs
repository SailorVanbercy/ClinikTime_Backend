using System.Security.Claims;
using ClinikTime.service.FichePatient;
using Infrastructure.user.Dto.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.controllers.FichePatient;
[ApiController]
[Route("api/v1/fiche-patient")]
public class FichePatientController(FichePatientService service) : ControllerBase
{
    //CREATE FICHE PATIENT
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create(CreateFichePatientDto dto)
    {
        var utilisateurId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var fiche = await service.CreateAsync(utilisateurId, dto);

        return CreatedAtAction(nameof(GetById), new { id = fiche.Id }, new
        {
            fiche.Id,
            fiche.Nom,
            fiche.Prenom,
            fiche.DateNaissance,
            fiche.Sexe,
            fiche.LienParente
        });
    }
    // ======================
    // GET FICHE BY ID
    // ======================
    [Authorize]
    [HttpGet("{id:int}")]
    public async Task<ActionResult> GetById(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var fiche = await service.GetByIdAsync(id);
        if (fiche == null)
            return NotFound();
        if(fiche.UtilisateurId != userId)
            return Forbid();

        return Ok(new
        {
            fiche.Id,
            fiche.Nom,
            fiche.Prenom,
            fiche.DateNaissance,
            fiche.Sexe,
            fiche.LienParente
        });
    }
    // ======================
    // GET ALL MY FICHES
    // ======================
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult> GetMyFiches()
    {
        var utilisateurId = int.Parse(
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value
        );

        var fiches = await service.GetByUtilisateurIdAsync(utilisateurId);

        return Ok(fiches.Select(f => new
        {
            f.Id,
            f.Nom,
            f.Prenom,
            f.DateNaissance,
            f.Sexe,
            f.LienParente
        }));
    }
}