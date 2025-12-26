namespace Infrastructure.user.EF.RendezVous;

public interface IRendezVousRepository
{
    Task<List<Domain.models.RendezVous>> GetByMedecinIdAsync(int medecinId);
    Task AddAsync(Domain.models.RendezVous rendezVous);
    Task<bool> ExistsOverlappingAsync(int medecinId, DateTime debut, DateTime fin);
    Task<Domain.models.RendezVous?> GetByidAsync(int id);
    Task UpdateAsync(Domain.models.RendezVous rendezVous);
    Task<bool> ExistsOverlappingExceptAsync(int rdvId, int medecinId, DateTime debut, DateTime fin);
    Task<List<Domain.models.RendezVous>> GetByUtilisateurIdAsync(int utilisateurId);
    Task<List<Domain.models.RendezVous>> GetPourJourAsync(int medecinId, DateTime date);
}