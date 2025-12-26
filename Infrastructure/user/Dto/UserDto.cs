using Domain.models;

namespace Infrastructure.user.Dto;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime DateCreation { get; set; }
    public Medecin? Medecin { get; set; }

    public UserDto(Utilisateur utilisateur)
    {
        this.Id = utilisateur.Id;
        this.Email = utilisateur.Email;
        this.Role = utilisateur.Role;
        this.DateCreation = utilisateur.DateCreation;
        this.Medecin = utilisateur.Medecin;
    }
}