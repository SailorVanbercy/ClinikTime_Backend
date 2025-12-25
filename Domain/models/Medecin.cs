namespace Domain.models;

public class Medecin
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;
    public string Prenom { get; set; } = null!;
    public string? Telephone { get; set; } //pas forcément de num référencé
    
    //FK vers Utilisateur
    public int UtilisateurId { get; set; }
    public Utilisateur Utilisateur { get; set; }
    
    //FK vers Specialité
    public int SpecialiteId { get; set; }
    public Specialite Specialite { get; set; }
    
    //Disponibilitées
    public ICollection<DisponibiliteMedecin> Disponibilites { get; set; } = new List<DisponibiliteMedecin>();
    
    //Rendez-vous
    public ICollection<RendezVous> RendezVous { get; set; } = new List<RendezVous>();
}