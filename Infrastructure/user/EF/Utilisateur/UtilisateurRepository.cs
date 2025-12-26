using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Domain.models;
namespace Infrastructure.user.EF;

public class UtilisateurRepository(ClinikTimeDbContext context) : IUtilisateurRepository
{
    public Task<List<Utilisateur>> GetAllAsync()
    {
        return  context.Utilisateurs.ToListAsync();
    }

    public Task<Utilisateur?> GetByEmailAsync(string email)
        => context.Utilisateurs.FirstOrDefaultAsync(u => u.Email == email);


    public Task<Utilisateur?> GetByIdAsync(int id)
        => context.Utilisateurs.Include(u => u.Medecin).FirstOrDefaultAsync(u => u.Id == id);

    public Task<Utilisateur?> GetByIdWithProfile(int id)
        =>  context.Utilisateurs
            .Include(u => u.Medecin)
                .ThenInclude(m => m.Specialite)
            .FirstAsync(u => u.Id == id);
    

    public async Task AddAsync(Utilisateur utilisateur)
    {
         await context.Utilisateurs.AddAsync(utilisateur);
         await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Utilisateur utilisateur)
    {
        context.Utilisateurs.Update(utilisateur);
        await context.SaveChangesAsync();
    }

    public void SaveChanges()
    {
        context.SaveChanges();
    }
}