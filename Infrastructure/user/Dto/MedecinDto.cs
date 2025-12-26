using Domain.models;

namespace Infrastructure.user.Dto;

public class MedecinDto
{
    public int Id { get; set; }
    public int UtilisateurId { get; set; }

    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public string? Telephone { get; set; }

    public int SpecialiteId { get; set; }

    public MedecinDto(Medecin medecin)
    {
        Id = medecin.Id;
        UtilisateurId = medecin.UtilisateurId;
        Nom = medecin.Nom;
        Prenom = medecin.Prenom;
        Telephone = medecin.Telephone;
        SpecialiteId = medecin.SpecialiteId;
    }
    public MedecinDto(){}
}