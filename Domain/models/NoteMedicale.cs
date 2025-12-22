namespace Domain.models;

public class NoteMedicale
{
    public int Id { get; set; }

    public DateTime Date { get; set; }
    public string Contenu { get; set; } = null!;

    public int MedecinId { get; set; }
    public int FichePatientId { get; set; }
}