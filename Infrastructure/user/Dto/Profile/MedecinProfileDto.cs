namespace Infrastructure.user.Dto.Profile;

public class MedecinProfileDto : UserProfileDto
{
    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public string? Telephone { get; set; }
    
    public int SpecialiteId { get; set; }
    public string Specialite { get; set; } = null!;
    
    public int DureeRdvMinutes { get; set; }

}