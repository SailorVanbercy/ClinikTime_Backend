using Domain.models;
using Infrastructure.user.Dto.Disponibilite;
using Infrastructure.user.EF;
using Infrastructure.user.EF.Disponibilite;

namespace ClinikTime.service.Disponibilite;

public class DisponibiliteMedecinService(IDisponibiliteMedecinRepository repository, IUtilisateurRepository utilisateurRepository)
{
    public async Task OuvrirAsync(int medecinId, BloquerDisponibiliteDto dto)
    {
        if(dto.Fin <= dto.Debut)
            throw new Exception("Dates Invalides");
        var dispo = new DisponibiliteMedecin
        {
            MedecinId = medecinId,
            Debut = dto.Debut,
            Fin = dto.Fin,
            EstBloquee = false
        };

        await repository.AddAsync(dispo);
    }

    public async Task BloquerAsync(int medecinId, BloquerDisponibiliteDto dto)
    {
        if(dto.Fin <= dto.Debut)
            throw new Exception("Dates Invalides");
        var dispo = new DisponibiliteMedecin
        {
            MedecinId = medecinId,
            Debut = dto.Debut,
            Fin = dto.Fin,
            EstBloquee = true
        };

        await repository.AddAsync(dispo);
    }

    public async Task<List<DisponibiliteDto>> GetMyDispo(int userId)
    {
        var user = await utilisateurRepository.GetByIdAsync(userId);
        if(user == null)
            throw new Exception("User not found");
        if(user.Medecin == null)
            throw new Exception("User is not a medecin");
        var medecin = user.Medecin;
        var medecinId = medecin.Id;

        var dispos = await repository.GetByMedecinIdAsync(medecinId);

        return dispos.Select(d => new DisponibiliteDto
        {
            Debut = d.Debut,
            Fin = d.Fin,
            EstBloque = d.EstBloquee
        }).ToList();
    }

    public async Task DeleteAsync(int dispoId, int medecinId)
    {
        var dispo = await repository.GetByIdAsync(dispoId);
        
        if(dispo == null)
            throw new Exception("Disponibilite not found");
        
        if(dispo.MedecinId != medecinId)
            throw new Exception("Acces interdit à cette disponibilité");

        await repository.DeleteAsync(dispoId, medecinId);
    }
}