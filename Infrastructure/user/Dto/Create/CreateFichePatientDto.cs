using System.ComponentModel.DataAnnotations;

namespace Infrastructure.user.Dto.Create;

public class CreateFichePatientDto
{
    [Required]
    public string Nom { get; set; } = null!;

    [Required]
    public string Prenom { get; set; } = null!;

    [Required]
    public DateTime DateNaissance { get; set; }

    public string? Sexe { get; set; }

    [Required]
    public string LienParente { get; set; } = null!;
}