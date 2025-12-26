using Infrastructure.user.Dto.Create;
using Infrastructure.user.EF.FichePatient;

namespace ClinikTime.service.FichePatient;

public class FichePatientService(IFichePatientRepository repository)
{
    //CREATE
    public async Task<Domain.models.FichePatient> CreateAsync(int utilisateurId, CreateFichePatientDto dto)
    {
        var fichePatient = new Domain.models.FichePatient
        {
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            DateNaissance = dto.DateNaissance,
            Sexe = dto.Sexe,
            LienParente = dto.LienParente,
            UtilisateurId = utilisateurId
        };
        await repository.AddAsync(fichePatient);
        return fichePatient;
    }
    
    //GET BY USER
    public async Task<List<Domain.models.FichePatient>> GetByUtilisateurIdAsync(int utilisateurId)
    {
        return await repository.GetByUtilisateurIdAsync(utilisateurId);
    }
    
    //GET BY ID
    public async Task<Domain.models.FichePatient?> GetByIdAsync(int id)
    {
        return await repository.GetByIdAsync(id);
    }
}