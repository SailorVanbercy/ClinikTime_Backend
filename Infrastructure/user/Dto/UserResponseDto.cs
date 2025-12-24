using Domain.models;

namespace Infrastructure.user.Dto;

public class UserResponseDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;

    public UserResponseDto(Utilisateur user)
    {
        Id = user.Id;
        Email = user.Email;
        Role = user.Role;
    }
}