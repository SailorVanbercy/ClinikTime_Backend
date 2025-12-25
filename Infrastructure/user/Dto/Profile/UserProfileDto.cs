namespace Infrastructure.user.Dto.Profile;

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string Role { get; set; } = null!;
    public DateTime DateCreation { get; set; }
}