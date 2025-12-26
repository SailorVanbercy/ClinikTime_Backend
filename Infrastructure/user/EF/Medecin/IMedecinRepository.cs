using Infrastructure.user.Dto.Create;

namespace Infrastructure.user.EF.Medecin;

public interface IMedecinRepository
{
    Task CreateMedecin(Domain.models.Medecin medecin);
    Task<Domain.models.Medecin?> GetByUtilisateurIdAsync(int utilisateurId);
    Task<Domain.models.Medecin?> GetByIdAsync(int medecinId);
    Task<List<Domain.models.Medecin>> GetAllAsync(int? specialiteId);
    Task<Domain.models.Medecin?> GetByIdWithSpecialiteAsync(int medecinId);
}