using Azure;
using ClinikTime.utils.PasswordHasher;
using Domain.models;
using Infrastructure.user.Dto;
using Infrastructure.user.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.service.Auth;

public class AuthService(IUtilisateurRepository repository, IPasswordHasher hasher) : IAuthService
{
    public async Task<Utilisateur?> LoginAsync(LoginUserDto dto)
    {
        var user = await repository.GetByEmailAsync(dto.Email);
        if (user == null)
        {
            return null;
        }

        if (!hasher.Verify(dto.MotDePasse, user.MotDePasseHash))
            return null;
        return user;
    }
}