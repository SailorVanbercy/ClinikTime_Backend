namespace Domain.models;

public class PasswordResetToken
{
    public int Id { get; set; }
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
    public bool Utilise { get; set; }
    public int UtilisateurId { get; set; }
}