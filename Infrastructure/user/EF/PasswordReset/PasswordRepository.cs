using Domain.models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.user.EF.PasswordReset;

public class PasswordRepository(ClinikTimeDbContext context) : IPasswordRepository
{
    public async Task AddAsync(PasswordResetToken token)
    {
        context.PasswordResetTokens.Add(token);
        await context.SaveChangesAsync();
    }

    public async Task<PasswordResetToken?> GetValidTokenAsync(string token)
    {
        return await context.PasswordResetTokens.FirstOrDefaultAsync(t =>
            t.Token == token &&
            !t.Utilise && 
            t.Expiration > DateTime.UtcNow);
    }

    public async Task InvalidateAllForUserAsync(int utilisateurId)
    {
        var tokens = await context.PasswordResetTokens
            .Where(t => t.UtilisateurId == utilisateurId && !t.Utilise)
            .ToListAsync();

        foreach (var token in tokens)
            token.Utilise = true;
    }

    public async Task UpdateAsync(PasswordResetToken token)
    {
        context.PasswordResetTokens.Update(token);
    }

    public async Task SaveAsync()
    {
        await context.SaveChangesAsync();
    }
}