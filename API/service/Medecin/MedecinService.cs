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
            Utilisateur = user,
            Nom = dto.Nom,
            Prenom = dto.Prenom,
            Telephone = dto.Telephone,
            SpecialiteId = dto.SpecialiteId
        };
        user.Medecin = medecin;
        
       await repository.CreateMedecin(medecin);
       await utilisateurRepository.UpdateAsync(user);
       return new MedecinDto
       {
           Id = medecin.Id,
           SpecialiteId = medecin.SpecialiteId,
           Nom = medecin.Nom,
           Prenom = medecin.Prenom,
           Telephone = medecin.Telephone,
           UtilisateurId = user.Id
       };
    }

    public async Task<MedecinDto?> GetMyMedecin(int id)
    {
        var medecin = await repository.GetByUtilisateurIdAsync(id);
        if (medecin == null)
            return null;
        return new MedecinDto(medecin);
    }

    public async Task<List<MedecinDto>> GetAllAsync(string? specialite)
    {
        // 🔹 Aucune spécialité → tous les médecins
        if (string.IsNullOrWhiteSpace(specialite))
        {
            var medecins = await repository.GetAllAsync();
            return medecins.Select(m => new MedecinDto(m)).ToList();
        }

        // 🔹 Spécialité fournie → on résout le nom
        var s = await repository.GetSpecialiteByNom(specialite);

        if (s == null)
            throw new Exception("Spécialité introuvable");

        var medecinsFiltres = await repository.GetAllAsync(s.Id);

        return medecinsFiltres.Select(m => new MedecinDto(m)).ToList();
    }


    public async Task<MedecinDto?> GetByUtilisateurIdAsync(int id)
    {
        var medecin =  await repository.GetByUtilisateurIdAsync(id);
        if (medecin == null)
            return null;
        return new  MedecinDto(medecin);
    }
    
}