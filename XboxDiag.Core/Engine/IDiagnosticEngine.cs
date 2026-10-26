using XboxDiag.Core.Models;

namespace XboxDiag.Core.Engine;

/// <summary>
/// Moteur d'inférence et d'orchestration pour le diagnostic technique de consoles Xbox.
/// </summary>
public interface IDiagnosticEngine
{
    /// <summary>
    /// Initialise une nouvelle session de diagnostic avec les symptômes observés.
    /// Calcule les premières hypothèses pondérées par les symptômes.
    /// </summary>
    /// <param name="modeleConsoleId">Identifiant du modèle de la console.</param>
    /// <param name="symptomeIds">Liste des identifiants des symptômes initiaux.</param>
    /// <returns>La session de diagnostic créée.</returns>
    SessionDiagnostic DemarrerSession(int modeleConsoleId, List<int> symptomeIds);

    /// <summary>
    /// Récupère la liste des hypothèses associées à une session, triées par score de confiance décroissant.
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    /// <returns>Liste des hypothèses ordonnées.</returns>
    List<SessionHypothese> ObtenirHypotheses(int sessionId);

    /// <summary>
    /// Détermine le prochain test optimal à réaliser pour discriminer efficacement les hypothèses restantes.
    /// Privilégie la facilité d'exécution (Difficulté Débutant avant Intermédiaire/Expert).
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    /// <returns>Le test recommandé ou null si aucun test pertinent n'est disponible.</returns>
    TestDiagnostic? ObtenirProchainTest(int sessionId);

    /// <summary>
    /// Enregistre le résultat d'un test appliqué et recalcule les statuts et scores de confiance des hypothèses.
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    /// <param name="testId">Identifiant du test réalisé.</param>
    /// <param name="resultat">Résultat obtenu (Conforme, NonConforme, etc.).</param>
    /// <param name="notes">Notes ou mesures relevées par le technicien.</param>
    void EnregistrerResultatTest(int sessionId, int testId, ResultatTest resultat, string? notes);

    /// <summary>
    /// Réévalue l'ensemble des hypothèses en appliquant les règles de diagnostic basées sur l'historique des tests.
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    void MettreAJourHypotheses(int sessionId);

    /// <summary>
    /// Ajoute un symptôme complémentaire en cours de diagnostic et réactualise les hypothèses.
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    /// <param name="symptomeId">Identifiant du symptôme à ajouter.</param>
    void AjouterSymptome(int sessionId, int symptomeId);

    /// <summary>
    /// Annule le dernier test saisi pour cette session et recalcule l'état logique des hypothèses.
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    void AnnulerDernierTest(int sessionId);

    /// <summary>
    /// Réinitialise la session en effaçant tous les tests réalisés et en restaurant les hypothèses initiales.
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    void RecommencerDiagnostic(int sessionId);

    /// <summary>
    /// Charge une session de diagnostic existante avec toutes ses données reliées.
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    /// <returns>La session chargée ou null si introuvable.</returns>
    SessionDiagnostic? ReprendreSession(int sessionId);

    /// <summary>
    /// Retourne l'historique complet de toutes les sessions de diagnostic enregistrées.
    /// </summary>
    /// <returns>Liste des sessions de diagnostic.</returns>
    List<SessionDiagnostic> ObtenirHistorique();

    /// <summary>
    /// Clôture le diagnostic, passe la session en statut Terminé et crée le dossier de réparation.
    /// </summary>
    /// <param name="sessionId">Identifiant de la session.</param>
    /// <param name="composantsRepares">Composants changés ou réparés.</param>
    /// <param name="resultatReparation">Bilan après réparation.</param>
    /// <param name="notes">Remarques finales du technicien.</param>
    /// <returns>Le dossier de réparation créé.</returns>
    DossierReparation CloturerDossier(int sessionId, string? composantsRepares, string? resultatReparation, string? notes);
}
