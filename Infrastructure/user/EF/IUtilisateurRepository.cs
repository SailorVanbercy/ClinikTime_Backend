using Domain.models;

namespace Infrastructure.user.EF;

public interface IUtilisateurRepository
{
    Task<List<Utilisateur>> GetAllAsync();
    Task<Utilisateur?> GetByEmailAsync(string email);
    Task<Utilisateur?> GetByIdAsync(int id);
    Task AddAsync(Utilisateur utilisateur);
    Task UpdateAsync(Utilisateur utilisateur);
}