namespace Infrastructure.user.Dto;

public class ConfirmPasswordResetDto
{
    public string Token { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}