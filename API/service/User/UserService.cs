using ClinikTime.utils.PasswordHasher;
using Domain.models;
using Infrastructure.user.Dto;
using Infrastructure.user.EF;

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
        return new UserDto(user);
    }

    public async Task<UserDto?> GetByEmailAsync(string email)
    {
        var user = await repository.GetByEmailAsync(email);
        if (user == null)
            return null;
        return new UserDto(user);
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
}