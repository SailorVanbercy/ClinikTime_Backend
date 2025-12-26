using System.ComponentModel.DataAnnotations;

namespace Infrastructure.user.Dto.Create;

public class CreateRendezVousDto
{
    [Required]
    public int MedecinId { get; set; }

    [Required]
    public int FichePatientId { get; set; }

    [Required]
    public DateTime Debut { get; set; }
    
    public string? Motif { get; set; }
}