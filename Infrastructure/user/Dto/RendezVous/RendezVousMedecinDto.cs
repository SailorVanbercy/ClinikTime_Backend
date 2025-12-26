namespace Infrastructure.user.Dto.RendezVous;

public class RendezVousMedecinDto
{
    public int Id { get; set; }
    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }
    public string Statut { get; set; } = null!;
    public string? Motif { get; set; }

    public int FichePatientId { get; set; }
    public string PatientNom { get; set; } = null!;
    public string PatientPrenom { get; set; } = null!;
}