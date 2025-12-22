using Domain.models;

namespace Infrastructure.user.Dto;

public class UpdateUserDto
{
    public string Email { get; set; } = null!;

    public UpdateUserDto(Utilisateur utilisateur)
    {
        this.Email = utilisateur.Email;
    }
}