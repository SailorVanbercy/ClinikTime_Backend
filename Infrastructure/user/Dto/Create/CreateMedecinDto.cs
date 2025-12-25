namespace Infrastructure.user.Dto.Create;

public class CreateMedecinDto
{
    public int UtilisateurId { get; set; }

    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public string? Telephone { get; set; }

    public int SpecialiteId { get; set; }
}