namespace XboxDiag.Core.Models;

/// <summary>
/// Représente une session active ou archivée de diagnostic pour une console donnée.
/// </summary>
public class SessionDiagnostic
{
    /// <summary>
    /// Identifiant unique de la session de diagnostic.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant du modèle de la console examinée.
    /// </summary>
    public int ModeleConsoleId { get; set; }

    /// <summary>
    /// Modèle de la console.
    /// </summary>
    public ModeleConsole ModeleConsole { get; set; } = null!;

    /// <summary>
    /// Numéro de série de la console (figurant sur l'étiquette arrière ou le châssis).
    /// </summary>
    public string? NumeroSerie { get; set; }

    /// <summary>
    /// État cosmétique et physique général (chocs, poussière, traces d'oxydation).
    /// </summary>
    public string? EtatGeneral { get; set; }

    /// <summary>
    /// Historique des réparations ou interventions antérieures signalées.
    /// </summary>
    public string? ReparationsPrecedentes { get; set; }

    /// <summary>
    /// Date et heure de création de la session.
    /// </summary>
    public DateTime DateCreation { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Statut actuel de la session.
    /// </summary>
    public StatutSession Statut { get; set; } = StatutSession.EnCours;

    /// <summary>
    /// Remarques diverses du technicien.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Symptômes constatés au cours de cette session.
    /// </summary>
    public List<SessionSymptome> SessionSymptomes { get; set; } = new();

    /// <summary>
    /// Résultats des tests effectués lors de cette session.
    /// </summary>
    public List<SessionResultatTest> ResultatsTests { get; set; } = new();

    /// <summary>
    /// Hypothèses de panne générées et évaluées pour cette session.
    /// </summary>
    public List<SessionHypothese> Hypotheses { get; set; } = new();

    /// <summary>
    /// Dossier de réparation associé lorsque le diagnostic est finalisé.
    /// </summary>
    public DossierReparation? DossierReparation { get; set; }
}
