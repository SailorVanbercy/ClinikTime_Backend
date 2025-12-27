namespace Infrastructure.user.Dto.Disponibilite;

public class DisponibiliteDto
{
    public int Id { get; set; }
    public DateTime Debut { get; set; }
    public DateTime Fin { get; set; }
    public bool EstBloque { get; set; }
}