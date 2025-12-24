namespace Infrastructure.user.Dto;

public class LoginUserDto
{
    public string Email { get; set; } = null!;
    public string MotDePasse { get; set; } = null!;
}