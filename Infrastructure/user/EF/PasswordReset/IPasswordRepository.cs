using Domain.models;

namespace Infrastructure.user.EF.PasswordReset;

public interface IPasswordRepository
{
    Task AddAsync(PasswordResetToken token);

    Task<PasswordResetToken?> GetValidTokenAsync(string token);
    
    Task InvalidateAllForUserAsync(int utilisateurId);
    Task UpdateAsync(PasswordResetToken token);
    Task SaveAsync();
}