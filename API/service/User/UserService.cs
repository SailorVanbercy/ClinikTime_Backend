using ClinikTime.service.Medecin;
using ClinikTime.utils.PasswordHasher;
using Domain.models;
using Infrastructure.user.Dto;
using Infrastructure.user.Dto.Create;
using Infrastructure.user.Dto.Profile;
using Infrastructure.user.EF;
using Microsoft.AspNetCore.Mvc;

namespace ClinikTime.service;

public class UserService(IUtilisateurRepository repository, IPasswordHasher hasher, MedecinService medecinService) 
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

    public async Task PromoteUserToMedecin(int userId, PromoteToMedecinDto dto)
    {
        // 1. Récupérer l'utilisateur
        var user = await repository.GetByIdAsync(userId);
        if(user == null)
            throw new Exception("User not found");
        
        // 2. Changer le rôle en "Medecin"
        // C'est obligatoire car ton MedecinService vérifie : if(user.Role != "Medecin") throw...
        user.Role = "Medecin";
        await repository.UpdateAsync(user);

        // 3. Préparer le DTO pour ton MedecinService
        var createMedecinDto = new CreateMedecinDto
        {
            UtilisateurId = userId,
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Telephone = dto.Telephone,
            SpecialiteId = dto.SpecialiteId
        };

        // 4. Appeler ton service existant pour créer l'entité Médecin
        await medecinService.CreateMedecin(createMedecinDto);
    }
}