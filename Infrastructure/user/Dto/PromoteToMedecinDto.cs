namespace Infrastructure.user.Dto;

public class PromoteToMedecinDto
{
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string Telephone { get; set; } = string.Empty;
    public int SpecialiteId { get; set; }
}