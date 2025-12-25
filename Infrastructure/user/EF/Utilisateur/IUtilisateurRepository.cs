using Domain.models;

namespace Infrastructure.user.EF;

public interface IUtilisateurRepository
{
    Task<List<Utilisateur>> GetAllAsync();
    Task<Domain.models.Utilisateur?> GetByEmailAsync(string email);
    Task<Domain.models.Utilisateur?> GetByIdAsync(int id);
    Task<Domain.models.Utilisateur?> GetByIdWithProfile(int id);
    Task AddAsync(Domain.models.Utilisateur utilisateur);
    Task UpdateAsync(Domain.models.Utilisateur utilisateur);
    void SaveChanges();
}