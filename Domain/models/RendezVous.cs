namespace Domain.models;

public class RendezVous
{
    public int Id { get; set; }

    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }

    public string? Motif { get; set; }
    public string Statut { get; set; } = null!;

    public int MedecinId { get; set; }
    public int FichePatientId { get; set; }
}