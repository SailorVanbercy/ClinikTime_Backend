using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.user.EF.FichePatient;

public class FichePatientrepository(ClinikTimeDbContext context) : IFichePatientRepository
{
    public async Task<Domain.models.FichePatient?> GetByIdAsync(int id)
    {
        return await context.FichePatients.FirstOrDefaultAsync(fp => fp.Id == id);
    }

    public async Task<List<Domain.models.FichePatient>> GetByUtilisateurIdAsync(int utilisateurId)
    {
        return await context.FichePatients
            .Where(fp => fp.UtilisateurId == utilisateurId)
            .OrderBy(fp => fp.Nom)
            .ThenBy(fp => fp.Prenom)
            .ToListAsync();
    }

    public async Task AddAsync(Domain.models.FichePatient fichePatient)
    {
        context.FichePatients.Add(fichePatient);
        await context.SaveChangesAsync();
    }
}