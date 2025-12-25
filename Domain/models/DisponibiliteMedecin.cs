namespace Domain.models;

public class DisponibiliteMedecin
{
    public int Id { get; set; }

    public int MedecinId { get; set; }

    public Medecin Medecin { get; set; } = null!;

    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }

    public bool EstBloquee { get; set; }
}