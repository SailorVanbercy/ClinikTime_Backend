using Domain.models;

namespace Infrastructure.user.EF.Disponibilite;

public interface IDisponibiliteMedecinRepository
{
    // ➕ Ajouter une disponibilité (ouverte OU bloquée)
    // Utilisé quand un médecin définit ses horaires
    // ou quand il bloque un créneau (pause, congé, etc.)
    Task AddAsync(DisponibiliteMedecin dispo);

    // 🔍 Récupérer une disponibilité précise par son id
    // Utile pour modifier ou supprimer une dispo plus tard
    Task<DisponibiliteMedecin?> GetByIdAsync(int id);

    // 📋 Récupérer TOUTES les disponibilités d’un médecin
    // Sert à afficher son agenda (ou à debug)
    Task<List<DisponibiliteMedecin>> GetByMedecinIdAsync(int medecinId);

    // ❌ Supprimer une disponibilité
    // (ex: supprimer une plage horaire mal définie)
    Task DeleteAsync(int id);

    // ✅ Vérifier qu’il existe AU MOINS UNE disponibilité OUVERTE
    // qui couvre complètement le rendez-vous demandé
    // ➜ règle métier : le médecin doit travailler à ce moment-là
    Task<bool> ExisteDisponibiliteOuverteCouvrantAsync(
        int medecinId,
        DateTime debut,
        DateTime fin
    );

    // 🚫 Vérifier qu’il n’existe AUCUNE disponibilité BLOQUÉE
    // qui chevauche le rendez-vous demandé
    // ➜ règle métier : pas de pause / congé / absence
    Task<bool> ExisteDispoBloqueeChevauchanteAsync(
        int medecinId,
        DateTime debut,
        DateTime fin
    );

    Task<List<DisponibiliteMedecin>> GetOuvertesPourJourAsync(int medecinId, DateTime date);
    Task<List<DisponibiliteMedecin>> GetBloqueesPourJourAsync(int medecinId, DateTime date);
}