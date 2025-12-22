namespace Domain.models;

public class Medecin
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public string? Telephone { get; set; } //pas forcément de num référencé

    public int UtilisateurId { get; set; }
    public int SpecialiteId { get; set; }
}