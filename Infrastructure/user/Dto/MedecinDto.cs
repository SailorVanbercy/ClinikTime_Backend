using Domain.models;

namespace Infrastructure.user.Dto;

public class MedecinDto
{
    public int UtilisateurId { get; set; }

    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public string? Telephone { get; set; }

    public int SpecialiteId { get; set; }

    public MedecinDto(Medecin medecin)
    {
        UtilisateurId = medecin.UtilisateurId;
        Nom = medecin.Nom;
        Prenom = medecin.Prenom;
        Telephone = medecin.Telephone;
        SpecialiteId = medecin.SpecialiteId;
    }
}