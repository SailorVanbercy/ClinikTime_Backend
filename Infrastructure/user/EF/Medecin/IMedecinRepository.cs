using Infrastructure.user.Dto.Create;

namespace Infrastructure.user.EF.Medecin;

public interface IMedecinRepository
{
    Task CreateMedecin(Domain.models.Medecin medecin);
    Task<Domain.models.Medecin?> GetByUtilisateurIdAsync(int utilisateurId);
}