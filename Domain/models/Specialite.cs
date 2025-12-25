namespace Domain.models;

public class Specialite
{
    public int Id { get; set; }

    public string Nom { get; set; } = null!;
    public int DureeRdvMinutes { get; set; }
    
    public ICollection<Medecin> Medecins { get; set; } = new List<Medecin>();
}