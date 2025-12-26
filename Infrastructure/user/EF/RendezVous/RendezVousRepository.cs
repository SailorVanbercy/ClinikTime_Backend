using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.user.EF.RendezVous;

public class RendezVousRepository(ClinikTimeDbContext context) : IRendezVousRepository
{
    public async Task<List<Domain.models.RendezVous>> GetByMedecinIdAsync(int medecinId)
    {
        return await context.RendezVous.Include(r => r.FichePatient)
            .Where(r => r.MedecinId == medecinId)
            .OrderBy(r => r.Debut)
            .ToListAsync();
    }

    public async Task AddAsync(Domain.models.RendezVous rendezVous)
    {
        context.RendezVous.Add(rendezVous);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExistsOverlappingAsync(int medecinId, DateTime debut, DateTime fin)
    {
        return await context.RendezVous.AnyAsync(r => r.MedecinId == medecinId
                                                      && debut < r.Fin && fin > r.Debut);
    }

    public async Task<Domain.models.RendezVous?> GetByidAsync(int id)
    {
        return await context.RendezVous.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task UpdateAsync(Domain.models.RendezVous rendezVous)
    {
        context.RendezVous.Update(rendezVous);
        await context.SaveChangesAsync();
    }
    public async Task<bool> ExistsOverlappingExceptAsync(
        int rendezVousId,
        int medecinId,
        DateTime debut,
        DateTime fin
    )
    {
        return await context.RendezVous.AnyAsync(r =>
            r.Id != rendezVousId &&
            r.MedecinId == medecinId &&
            r.Debut < fin &&
            r.Fin > debut
        );
    }

    public async Task<List<Domain.models.RendezVous>> GetByUtilisateurIdAsync(int utilisateurId)
    {
        return await context.RendezVous
            .Include(r => r.Medecin)
            .ThenInclude(m => m.Specialite)
            .Where(r =>
                context.FichePatients.Any(fp => fp.Id == r.FichePatientId && fp.UtilisateurId == utilisateurId))
            .OrderBy(r => r.Debut)
            .ToListAsync();
    }

    public async Task<List<Domain.models.RendezVous>> GetPourJourAsync(int medecinId, DateTime date)
    {
        var debutJour = date.Date;
        var finJour = debutJour.AddDays(1);
        return await context.RendezVous
            .Where(r =>
                r.MedecinId == medecinId &&
                r.Debut < finJour &&
                r.Fin > debutJour &&
                r.Statut != "Annule" &&
                r.Statut != "Refuse")
            .OrderBy(r => r.Debut)
            .ToListAsync();
    }

    public Task DeleteAsync(Domain.models.RendezVous rendezVous)
    {
         context.RendezVous.Remove(rendezVous);
         context.SaveChanges();
         return Task.CompletedTask;
    }
}