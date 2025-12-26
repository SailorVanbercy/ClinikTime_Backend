namespace Infrastructure.user.EF.FichePatient;

public interface IFichePatientRepository
{
    Task<Domain.models.FichePatient?> GetByIdAsync(int id);

    Task<List<Domain.models.FichePatient>> GetByUtilisateurIdAsync(int utilisateurId);

    Task AddAsync(Domain.models.FichePatient fichePatient);
}