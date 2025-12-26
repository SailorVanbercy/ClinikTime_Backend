namespace Domain.models;

public class RendezVous
{
    public int Id { get; set; }

    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }

    public string? Motif { get; set; }
    public string Statut { get; set; } = "Confirme";

    public int MedecinId { get; set; }
    public Medecin Medecin { get; set; } = null!;
    public int FichePatientId { get; set; }
    public FichePatient FichePatient { get; set; } = null!;
}