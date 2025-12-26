using System.Security.Claims;
using ClinikTime.service;
using ClinikTime.service.FichePatient;
using ClinikTime.service.RendezVous;
using Infrastructure.user.Dto;
using Infrastructure.user.Dto.Create;
using Infrastructure.user.Dto.RendezVous;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.controllers.RendezVous;
[ApiController]
[Route("api/v1/rendezvous")]
public class RendezVousController(RendezVousService service, UserService userService, FichePatientService fichePatientService) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<ActionResult> Create(CreateRendezVousDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var fiche = await fichePatientService.GetByIdAsync(dto.FichePatientId);
        if (fiche == null)
            throw new Exception("la Fiche patient n'existe pas");
        if (userId != fiche.UtilisateurId)
            return Forbid();
        var rendezVous = await service.CreateAsync(dto);
        return Ok(new
        {
            rendezVous.Id,
            rendezVous.Debut,
            rendezVous.Fin,
            rendezVous.Statut
        });
    }

    [Authorize]
    [HttpGet("medecin/getMyRendezVous")]
    public async Task<ActionResult> GetByMedecinId()
    {
         var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
         var user = await userService.GetByIdAsync(userId);
         if (user == null)
             return NotFound();
         if (user.Medecin == null)
             throw new Exception("you are not a Medecin");
         
         var rendezVous = await service.GetByMedecinId(user.Medecin.Id);
         return Ok(rendezVous.Select(r => new RendezVousMedecinDto
         {
             Id = r.Id,
             Debut = r.Debut,
             Fin = r.Fin,
             FichePatientId = r.FichePatientId,
             Motif = r.Motif,
             PatientNom = r.FichePatient.Nom,
             PatientPrenom = r.FichePatient.Prenom,
             Statut = r.Statut
         }));
    }

    [Authorize]
    [HttpPut("{id}/annuler")]
    public async Task<ActionResult> Annuler(int id)
    {
        var userid = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await service.AnnulerAsync(id, userid);
        return Ok("Rendez-vous annulé");
    }

    [Authorize]
    [HttpPut("{id}/modifier")]
    public async Task<ActionResult> Modifier(int id, UpdateRendezVousDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        await service.ModifierAsync(id, userId, dto);
        return Ok("Rendez-vous modifié");
    }

    [Authorize(Roles = "Medecin")]
    [HttpPut("{id}/refuser")]
    public async Task<ActionResult> Refuser(int id)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await userService.GetByIdAsync(userId);
        if(user == null)
            return NotFound();
        var medecin = user.Medecin;
        if (medecin == null)
            throw new Exception("L'utilisateur connecté n'est pas un médecin");
        var medecinId = medecin.Id;
        await service.RefuserAsync(id, medecinId);
        return Ok("Rendez-vous refusé");
    }

    [Authorize(Roles = "Medecin")]
    [HttpPut("{id}/reprogrammer")]
    public async Task<ActionResult> Reprogrammer(int id, ReprogrammerRendezVousDto dto)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var user = await userService.GetByIdAsync(userId);
        if(user == null)
            return NotFound();
        var medecin = user.Medecin;
        if (medecin == null)
            throw new Exception("L'utilisateur connecté n'est pas un médecin");
        var medecinId = medecin.Id;

        await service.ReprogrammerAsync(id, medecinId, dto);
        return Ok("Rendez-vous reprogrammé");
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<List<RendezVousPatientDto>>> GetMyRendezVous()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        
        var rdvs = await service.GetForPatientAsync(userId);
        return Ok(rdvs);
    }

    [Authorize]
    [HttpGet("{id}/creneaux-libres")]
    public async Task<ActionResult<List<CreneauLibreDto>>> GetCreneauLibre(int id, [FromQuery] DateTime date)
    {
        var creneaux = await service.GetCreneauxLibresAsync(id, date);
        return Ok(creneaux);
    }
}
