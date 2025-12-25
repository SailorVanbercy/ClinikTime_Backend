using Domain.models;

namespace Infrastructure.user.Dto;

public class CreateUserDto
{
    public string Email { get; set; } = null!;
    public string MotDePasse { get; set; } = null!;
    
}