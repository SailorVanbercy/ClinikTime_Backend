namespace Domain.models;

public class FichePatient
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public DateTime DateNaissance { get; set; }

    public string Sexe { get; set; } = null!;
    public string LienParente { get; set; } = null!;

    public int UtilisateurId { get; set; }
}