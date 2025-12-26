namespace Infrastructure.user.Dto.RendezVous;

public class RendezVousPatientDto
{
    public int Id { get; set; }
    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }
    public string Statut { get; set; } = null!;
    public string? Motif { get; set; }

    public int MedecinId { get; set; }
    public string MedecinNom { get; set; } = null!;
    public string MedecinPrenom { get; set; } = null!;
    public string Specialite { get; set; } = null!;
}