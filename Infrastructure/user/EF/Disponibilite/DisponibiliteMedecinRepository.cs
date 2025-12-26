using Domain.models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.user.EF.Disponibilite;

public class DisponibiliteMedecinRepository(ClinikTimeDbContext context) : IDisponibiliteMedecinRepository
{
    public async Task AddAsync(DisponibiliteMedecin dispo)
    {
        context.DisponibilitesMedecin.Add(dispo);
        await context.SaveChangesAsync();
    }

    public async Task<DisponibiliteMedecin?> GetByIdAsync(int id)
    {
        return await context.DisponibilitesMedecin.FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task<List<DisponibiliteMedecin>> GetByMedecinIdAsync(int medecinId)
    {
        return await context.DisponibilitesMedecin
            .Where(d => d.MedecinId == medecinId)
            .OrderBy(d => d.Debut)
            .ToListAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var dispo = await context.DisponibilitesMedecin.FirstOrDefaultAsync(d => d.Id == id);

        if (dispo == null)
            throw new Exception("Disponibilite inexistente");
        context.DisponibilitesMedecin.Remove(dispo);
        await context.SaveChangesAsync();
    }

    public async Task<bool> ExisteDisponibiliteOuverteCouvrantAsync(int medecinId, DateTime debut, DateTime fin)
    {
        return await context.DisponibilitesMedecin.AnyAsync(d =>
            d.MedecinId == medecinId &&
            d.EstBloquee == false &&
            d.Debut <= debut &&
            d.Fin >= fin
        );
    }

    public async Task<bool> ExisteDispoBloqueeChevauchanteAsync(int medecinId, DateTime debut, DateTime fin)
    {
        return await context.DisponibilitesMedecin.AnyAsync(d =>
            d.MedecinId == medecinId &&
            d.EstBloquee == true &&
            d.Debut < fin &&
            d.Fin > debut
        );
    }

    public async Task<List<DisponibiliteMedecin>> GetOuvertesPourJourAsync(int medecinId, DateTime date)
    {
        var debutJour = date.Date;
        var finJour = debutJour.AddDays(1);

        return await context.DisponibilitesMedecin
            .Where(d => d.MedecinId == medecinId &&
                        !d.EstBloquee && d.Debut < finJour &&
                        d.Fin > debutJour)
            .OrderBy(d => d.Debut)
            .ToListAsync();
    }
    

    public async Task<List<DisponibiliteMedecin>> GetBloqueesPourJourAsync(int medecinId, DateTime date)
    {
        var debutJour = date.Date;
        var finJour = debutJour.AddDays(1);

        return await context.DisponibilitesMedecin
            .Where(d => d.MedecinId == medecinId &&
                        d.EstBloquee && d.Debut < finJour &&
                        d.Fin > debutJour)
            .OrderBy(d => d.Debut)
            .ToListAsync();
    }
}