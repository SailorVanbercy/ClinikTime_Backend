using ClinikTime.service.Email;
using ClinikTime.utils;
using Domain.models;
using Infrastructure.user.EF;
using Infrastructure.user.EF.PasswordReset;

namespace ClinikTime.service.PasswordReset;

public class PasswordResetService(IUtilisateurRepository utilisateurRepository, IPasswordRepository passwordRepository, IEmailService emailService)
{
    public async Task RequestResetAsync(string email)
    {
        var user = await utilisateurRepository.GetByEmailAsync(email);
        if (user == null)
            return;

        var token = new PasswordResetToken
        {
            Token = Guid.NewGuid().ToString(),
            UtilisateurId = user.Id,
            Expiration = DateTime.UtcNow.AddMinutes(30),
            Utilise = false
        };

        await passwordRepository.AddAsync(token);
        
        var resetLink = $"http://localhost:4200/reset-password?token={token.Token}";

        await emailService.SendEmailAsync(user.Email, "Réinitialisation du mot de passe",
            $"Clique sur ce lien pour réinitialiser ton mot de passe {resetLink}");
    }

    public async Task ConfirmResetAsync(string tokenValue, string newPassword)
    {
        var token = await passwordRepository.GetValidTokenAsync(tokenValue);
        if (token == null)
            throw new Exception("Token invalide ou expiré");

        var user = await utilisateurRepository.GetByIdAsync(token.UtilisateurId);
        if (user == null)
            throw new Exception("Utilisateur Introuvable");
        
        //Hash du nouveau mdp
        user.MotDePasseHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        
        //Invalider le token
        token.Utilise = true;

        await utilisateurRepository.UpdateAsync(user);
        await passwordRepository.SaveAsync();
    }
}