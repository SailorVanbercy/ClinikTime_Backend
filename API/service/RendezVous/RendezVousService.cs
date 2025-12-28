using ClinikTime.service.Email;
using Infrastructure.user.Dto;
using Infrastructure.user.Dto.Create;
using Infrastructure.user.Dto.RendezVous;
using Infrastructure.user.EF;
using Infrastructure.user.EF.Disponibilite;
using Infrastructure.user.EF.FichePatient;
using Infrastructure.user.EF.Medecin;
using Infrastructure.user.EF.RendezVous;

namespace ClinikTime.service.RendezVous;

public class RendezVousService(IRendezVousRepository repository, IMedecinRepository medecinRepository, 
    IFichePatientRepository patientRepository, IDisponibiliteMedecinRepository disponibiliterepository, IUtilisateurRepository userRepository, IEmailService emailService)
{
    public async Task<Domain.models.RendezVous> CreateAsync(CreateRendezVousDto dto)
    {
        //Médecin Inexistant
        var medecin = await medecinRepository.GetByIdWithSpecialiteAsync(dto.MedecinId);
        if (medecin == null)
            throw new Exception("Médecin introuvable");
        var duree = medecin.Specialite.DureeRdvMinutes;
        var fin = dto.Debut.AddMinutes(duree);
        
        // DATES INVALIDES
        if (fin <= dto.Debut)
            throw new Exception("La date de fin doit être après la date de début");
        //FICHE PATIENT INEXISTANTE
        var fiche = await patientRepository.GetByIdAsync(dto.FichePatientId);
        if(fiche == null)
            throw new Exception("Fiche Patient inexistente");
        
        //VERIFICATION DISPONIBILITE MEDECIN
        var dispoOuverte = await disponibiliterepository.ExisteDisponibiliteOuverteCouvrantAsync(dto.MedecinId, dto.Debut, fin);
        if (!dispoOuverte)
            throw new Exception("Le medecin n'est pas disponible à ce créneau là");
        
        var bloquee = await disponibiliterepository.ExisteDispoBloqueeChevauchanteAsync(dto.MedecinId, dto.Debut, fin);
        if(bloquee)
            throw new Exception("ce créneau est bloqué");
        
        
        // Chevauchement pour ce medecin
        var overlap = await repository.ExistsOverlappingAsync(dto.MedecinId, dto.Debut, fin);
        if (overlap)
            throw new Exception("Ce créneau n'est pas disponible");
        
        //Creation du Rendez vous
        var rendezVous = new Domain.models.RendezVous
        {
            MedecinId = dto.MedecinId,
            FichePatientId = dto.FichePatientId,
            Debut = dto.Debut,
            Fin = fin,
            Motif = dto.Motif,
            Statut = "Confirme"
        };
        await repository.AddAsync(rendezVous);
        return rendezVous;
    }

    public async Task<List<Domain.models.RendezVous>> GetByMedecinId(int id)
    {
        return await repository.GetByMedecinIdAsync(id);
    }

    public async Task AnnulerAsync(int rendezVousId, int userid)
    {
        //Récupérer le rdv
        var rdv = await repository.GetByidAsync(rendezVousId);
        if (rdv == null)
            throw new Exception("Rendez-vous introuvable");
        //Récupérer la fiche patient
        var fiche = await patientRepository.GetByIdAsync(rdv.FichePatientId);
        if(fiche == null)
            throw new Exception("Fiche Patient introuvable");
        
        // Sécurité : le rdv doit appartenir au user
        if (fiche.UtilisateurId != userid)
            throw new Exception("La fiche utilisateur n'est pas aux user connecté");
        
        //rdv déjà annulé
        if (rdv.Statut == "Annule")
            throw new Exception("Rendez-Vous déjà annulé");
        
        //Annulation
        rdv.Statut = "Annule";
        await repository.UpdateAsync(rdv);
        await repository.DeleteAsync(rdv);

    }

    public async Task ModifierAsync(int rdvId, int userId, UpdateRendezVousDto dto)
    {
        //RDV EXISTE
        var rdv = await repository.GetByidAsync(rdvId);
        if(rdv == null)
            throw new Exception("Rendez-vous introuvable");
        
        //FICHE PATIENT EXISTE
        var fiche = await patientRepository.GetByIdAsync(rdv.FichePatientId);
        if (fiche == null)
            throw new Exception("Fiche Patient introuvable");
        
        //VERIFICATION D'APPARTENANCE
        if(fiche.UtilisateurId != userId)
            throw new UnauthorizedAccessException();
        
        //STATUT VALIDE ?
        if (rdv.Statut == "Annule" || rdv.Statut == "Refuse")
            throw new Exception("Ce rendez-vous ne peut plus être modifié");
        
        //RECUPERATION MEDECIN + SPECIALITE
        var medecin = await medecinRepository.GetByIdWithSpecialiteAsync(rdv.MedecinId);
        if (medecin == null || medecin.Specialite == null)
            throw new Exception("Médecin ou spécialité introuvable");
        
        //RECALCUL FIN RDV
        var nouvelleFin = dto.NouveauDebut.AddMinutes(medecin.Specialite.DureeRdvMinutes);
        
        //VERIFIER DISPO OUVERTE
        var dispoOuverte =
            await disponibiliterepository.ExisteDisponibiliteOuverteCouvrantAsync(rdv.MedecinId, dto.NouveauDebut,
                nouvelleFin);
        if (!dispoOuverte)
            throw new Exception("Le médecin n'est pas disponible pour ce créneau");
        
        //VERIFIER BLOCAGE
        var bloque =
            await disponibiliterepository.ExisteDispoBloqueeChevauchanteAsync(rdv.MedecinId, dto.NouveauDebut,
                nouvelleFin);
        if (bloque)
            throw new Exception("Ce Créneau est bloqué");
        
        //VERIFIER CONFLIT en excluant ce rdv
        var conflit =
            await repository.ExistsOverlappingExceptAsync(rdv.Id, rdv.MedecinId, dto.NouveauDebut, nouvelleFin);
        if (conflit)
            throw new Exception("Un autre rdv existe sur ce créneau");
        
        //MISE A JOUR
        rdv.Debut = dto.NouveauDebut;
        rdv.Fin = nouvelleFin;
        rdv.Motif = dto.Motif;

        await repository.UpdateAsync(rdv);
    }

    public async Task RefuserAsync(int rdvId, int medecinId)
    {
        var rdv = await repository.GetByidAsync(rdvId);
        if(rdv == null)
            throw new Exception("Rdv introuvable");
        if (rdv.MedecinId != medecinId)
            throw new UnauthorizedAccessException();
        if (rdv.Statut is "Refuse" or "Annule")
            throw new Exception("Ce rendez-vous ne peut plus êtr modifié");
        var medecin = await medecinRepository.GetByIdAsync(rdv.MedecinId);
        if(medecin == null)
            throw new Exception("Medecin introuvable");

        rdv.Statut = "Refuse";
        await repository.UpdateAsync(rdv);

        // --- ENVOI DE L'EMAIL AU PATIENT ---
        await EnvoyerNotificationAsync(
            rdv.FichePatientId,
            $"Rendez-vous annulé par le médecin {medecin.Nom}",
            $"Bonjour,<br/><br/>Votre rendez-vous du <b>{rdv.Debut}</b> a été annulé par le docteur {medecin.Nom}."
        );

        await repository.DeleteAsync(rdv);
    }
    public async Task ReprogrammerAsync(
        int rendezVousId,
        int medecinId,
        ReprogrammerRendezVousDto dto
    )
    {
        var rdv = await repository.GetByidAsync(rendezVousId);
        if (rdv == null)
            throw new Exception("Rendez-vous introuvable");

        if (rdv.MedecinId != medecinId)
            throw new UnauthorizedAccessException();

        if (rdv.Statut == "Annule" || rdv.Statut == "Refuse")
            throw new Exception("Ce rendez-vous ne peut plus être modifié");

        var medecin = await medecinRepository
            .GetByIdWithSpecialiteAsync(rdv.MedecinId);

        if (medecin == null || medecin.Specialite == null)
            throw new Exception("Médecin ou spécialité introuvable");

        var nouvelleFin = dto.NouveauDebut
            .AddMinutes(medecin.Specialite.DureeRdvMinutes);
        

        // blocage
        var bloque = await disponibiliterepository
            .ExisteDispoBloqueeChevauchanteAsync(
                rdv.MedecinId,
                dto.NouveauDebut,
                nouvelleFin
            );

        if (bloque)
            throw new Exception("Créneau bloqué");

        // conflit RDV (exclure ce RDV)
        var conflit = await repository
            .ExistsOverlappingExceptAsync(
                rdv.Id,
                rdv.MedecinId,
                dto.NouveauDebut,
                nouvelleFin
            );

        if (conflit)
            throw new Exception("Conflit avec un autre rendez-vous");
        
        //sauvegarde de l'ancienne date
        var ancienneDate = rdv.Debut;
        rdv.Debut = dto.NouveauDebut;
        rdv.Fin = nouvelleFin;
        rdv.Statut = "Confirme";

        await repository.UpdateAsync(rdv);


        await EnvoyerNotificationAsync(rdv.FichePatientId,
            "Votre rendez-vous a été déplacé",
            $"Bonjour, <br/><br/>Votre rendez-vous initialement prévu le <b>{ancienneDate}</b> a été déplacé au <b>{rdv.Debut}</b>");
    }

    public async Task<List<RendezVousPatientDto>> GetForPatientAsync(int utilisateurId)
    {
        var rdvs =  await repository.GetByUtilisateurIdAsync(utilisateurId);
        return rdvs.Select(r => new RendezVousPatientDto
        {
            Id = r.Id,
            Debut = r.Debut,
            Fin = r.Fin,
            Statut = r.Statut,
            Motif = r.Motif,
            MedecinId = r.MedecinId,
            MedecinNom = r.Medecin.Nom,
            MedecinPrenom = r.Medecin.Prenom,
            Specialite = r.Medecin.Specialite.Nom
        }).ToList();
    }


    public async Task<List<CreneauLibreDto>> GetCreneauxLibresAsync(int medecinId, DateTime date)
    {
        // 1️⃣ récupérer le médecin + spécialité
        var medecin = await medecinRepository.GetByIdWithSpecialiteAsync(medecinId);
        if (medecin == null || medecin.Specialite == null)
            throw new Exception("Médecin ou spécialité introuvable");

        var duree = medecin.Specialite.DureeRdvMinutes;

        // 2️⃣ récupérer les données du jour
        var disposOuvertes = await disponibiliterepository
            .GetOuvertesPourJourAsync(medecinId, date);

        var disposBloquees = await disponibiliterepository
            .GetBloqueesPourJourAsync(medecinId, date);

        var rdvs = await repository.GetPourJourAsync(medecinId, date);

        var creneauxLibres = new List<CreneauLibreDto>();

        // 3️⃣ calcul
        foreach (var dispo in disposOuvertes)
        {
            var cursor = dispo.Debut;

            while (cursor.AddMinutes(duree) <= dispo.Fin)
            {
                var debut = cursor;
                var fin = cursor.AddMinutes(duree);

                var chevaucheBloque = disposBloquees.Any(b =>
                    b.Debut < fin && b.Fin > debut
                );

                var chevaucheRdv = rdvs.Any(r =>
                    r.Debut < fin && r.Fin > debut
                );

                if (!chevaucheBloque && !chevaucheRdv)
                {
                    creneauxLibres.Add(new CreneauLibreDto
                    {
                        Debut = debut,
                        Fin = fin
                    });
                }

                cursor = cursor.AddMinutes(duree);
            }
        }

        return creneauxLibres;
    }
    // --- Méthode Helper Privée pour l'envoi d'emails ---
    private async Task EnvoyerNotificationAsync(int fichePatientId, string subject, string body)
    {
        try
        {
            // 1. Récupérer la fiche patient pour avoir l'ID Utilisateur
            var fiche = await patientRepository.GetByIdAsync(fichePatientId);
            if (fiche == null) return; 

            // 2. Récupérer l'utilisateur pour avoir son email
            var user = await userRepository.GetByIdAsync(fiche.UtilisateurId);
            if (user == null || string.IsNullOrEmpty(user.Email)) return;

            // 3. Envoyer l'email
            await emailService.SendEmailAsync(user.Email, subject, body);
        }
        catch (Exception ex)
        {
            // On attrape l'exception pour ne pas bloquer l'action principale si le mail échoue
            Console.WriteLine($"Erreur lors de l'envoi de l'email : {ex.Message}");
        }
    }
}