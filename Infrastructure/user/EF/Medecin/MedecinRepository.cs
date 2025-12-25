using Infrastructure.Data;
using Domain.models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.user.EF.Medecin;

public class MedecinRepository(ClinikTimeDbContext context) : IMedecinRepository
{
    public async Task CreateMedecin(Domain.models.Medecin medecin)
    {
       await context.Medecins.AddAsync(medecin);
       context.SaveChanges();
    }

    public async Task<Domain.models.Medecin?> GetByUtilisateurIdAsync(int utilisateurId)
    {
        return await context.Medecins.FirstOrDefaultAsync(m => m.UtilisateurId == utilisateurId);
    }
}