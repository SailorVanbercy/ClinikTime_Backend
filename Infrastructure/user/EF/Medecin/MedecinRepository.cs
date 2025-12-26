using Infrastructure.Data;
using Domain.models;
using Infrastructure.user.Dto;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.user.EF.Medecin;

public class MedecinRepository(ClinikTimeDbContext context) : IMedecinRepository
{
    public async Task CreateMedecin(Domain.models.Medecin medecin)
    {
       await context.Medecins.AddAsync(medecin);
       await context.SaveChangesAsync();
    }

    public async Task<Domain.models.Medecin?> GetByUtilisateurIdAsync(int utilisateurId)
    {
        return await context.Medecins.FirstOrDefaultAsync(m => m.UtilisateurId == utilisateurId);
    }

    public async Task<Domain.models.Medecin?> GetByIdAsync(int medecinId)
    {
        return await context.Medecins.FirstOrDefaultAsync(m => m.Id == medecinId);
    }

    

    public async Task<List<Domain.models.Medecin>> GetAllAsync(int? specialiteId)
    {
        var query = context.Medecins
            .Include(m => m.Specialite)
            .AsQueryable();

        if (specialiteId.HasValue)
        {
            query = query.Where(m => m.SpecialiteId == specialiteId);
        }

        return await query
            .OrderBy(m => m.Nom)
            .ThenBy(m => m.Prenom)
            .ToListAsync();
    }

    public async Task<Domain.models.Medecin?> GetByIdWithSpecialiteAsync(int medecinId)
    {
        return await context.Medecins.Include(m => m.Specialite)
            .FirstOrDefaultAsync(m => m.Id == medecinId);
    }
}