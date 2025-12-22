using Domain.models;

namespace Infrastructure.user.Dto;

public class CreateUserDto
{
    public string Email { get; set; } = null!;
    public string MotDePasse { get; set; } = null!;

    public CreateUserDto(Utilisateur utilisateur)
    {
        this.Email = utilisateur.Email;
        this.MotDePasse = utilisateur.MotDePasseHash;
    }
}