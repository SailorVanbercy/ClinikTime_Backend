using ClinikTime.utils.PasswordHasher;
using Domain.models;
using Infrastructure.user.Dto;
using Infrastructure.user.Dto.Profile;
using Infrastructure.user.EF;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.service;

public class UserService(IUtilisateurRepository repository, IPasswordHasher hasher) 
{
    public async Task<List<UserDto>> GetAllAsync()
    {
        var users = await repository.GetAllAsync();
        return users.Select(u => new UserDto(u)).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(int id)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
            return null;
        Console.WriteLine(user.Medecin != null ? "Medecin Initialisé" : "Medecin pas initialisé");
        return new UserDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await repository.GetByEmailAsync(email);
        if (user == null)
            return null;
        return new UserDto(user);
    }

    public async Task<UserProfileDto?> GetMyProfileAsync(int id)
    {
        var user = await repository.GetByIdWithProfile(id);
        if (user == null)
            return null;
        
        //Cas Medecin
        if (user.Role == "Medecin" && user.Medecin != null)
        {
            return new MedecinProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                DateCreation = user.DateCreation,

                Nom = user.Medecin.Nom,
                Prenom = user.Medecin.Prenom,
                Telephone = user.Medecin.Telephone,

                SpecialiteId = user.Medecin.SpecialiteId,
                Specialite = user.Medecin.Specialite.Nom,
                DureeRdvMinutes = user.Medecin.Specialite.DureeRdvMinutes
            };
        }
        
        //Cas utilisateur
        return new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Role = user.Role,
            DateCreation = user.DateCreation
        };
    }

    public async Task<UserDto> AddAsync(CreateUserDto dto)
    {
        var user = new Utilisateur
        {
            Email = dto.Email,
            MotDePasseHash = hasher.Hash(dto.MotDePasse),
            Role = "User",
            DateCreation = DateTime.UtcNow
        };
        await repository.AddAsync(user);
        return new UserDto(user);
    }

    public async Task<bool> UpdateAsync(int id,UpdateUserDto dto)
    {
        var user = await repository.GetByIdAsync(id);
        if (user == null)
            return false;

        user.Email = dto.Email;

        await repository.UpdateAsync(user);
        return true;

    }

    public async Task<NoContentResult> PromoteUserToMedecin(int id)
    {
        var user = await repository.GetByIdAsync(id);
        if(user == null)
            throw new Exception("User not found");
        if(user.Role != "User")
            throw new Exception("User already has a role different of User Role");

        user.Role = "Medecin";
        
        repository.SaveChanges();
        return new NoContentResult();
    }
}