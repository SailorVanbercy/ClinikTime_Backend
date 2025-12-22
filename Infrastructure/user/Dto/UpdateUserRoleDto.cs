using Domain.models;

namespace Infrastructure.user.Dto;

public class UpdateUserRoleDto
{
    public string Role { get; set; } = null!;

    public UpdateUserRoleDto(Utilisateur utilisateur)
    {
        this.Role = utilisateur.Role;
    }
}