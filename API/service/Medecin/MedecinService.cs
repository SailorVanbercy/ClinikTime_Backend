using Infrastructure.user.Dto;
using Infrastructure.user.Dto.Create;
using Infrastructure.user.EF;
using Infrastructure.user.EF.Medecin;

namespace ClinikTime.service.Medecin;

public class MedecinService(IMedecinRepository repository, IUtilisateurRepository utilisateurRepository)
{
    public async Task<MedecinDto> CreateMedecin(CreateMedecinDto dto)
    {
        var user = await utilisateurRepository.GetByIdAsync(dto.UtilisateurId);
        if(user == null)
            throw new Exception("User Not Found");
        if(user.Role != "Medecin")
            throw new Exception("User Is not a Medecin");
        var existingMedecin = await repository.GetByUtilisateurIdAsync(dto.UtilisateurId);
        if(existingMedecin != null)
            throw new Exception("Medecin Already Exists");
        
        var medecin = new Domain.models.Medecin
        {
            UtilisateurId = dto.UtilisateurId,
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Telephone = dto.Telephone,
            SpecialiteId = dto.SpecialiteId
        };
       await repository.CreateMedecin(medecin);
       return new MedecinDto(medecin);
    }
}