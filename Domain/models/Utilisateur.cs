namespace Domain.models;

public class Utilisateur
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;
    public string MotDePasseHash { get; set; } = null!;
    public string Role { get; set; } = null!;

    public DateTime DateCreation { get; set; }
    
    //Relation Optionelle avec Medecin
    public Medecin? Medecin { get; set; }
    
    //Fiches Patient
    public List<FichePatient> FichesPatients { get; set; } = new List<FichePatient>();
}