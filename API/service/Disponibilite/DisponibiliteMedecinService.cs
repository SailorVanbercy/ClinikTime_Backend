using Domain.models;
using Infrastructure.user.Dto.Disponibilite;
using Infrastructure.user.EF.Disponibilite;

namespace ClinikTime.service.Disponibilite;

public class DisponibiliteMedecinService(IDisponibiliteMedecinRepository repository)
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
}