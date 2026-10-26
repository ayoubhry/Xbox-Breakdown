namespace XboxDiag.Core.Models;

/// <summary>
/// Clôture et archivage d'une intervention technique avec rapport de réparation.
/// </summary>
public class DossierReparation
{
    /// <summary>
    /// Identifiant unique du dossier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la session de diagnostic ayant abouti à ce dossier (relation 1-à-1).
    /// </summary>
    public int SessionDiagnosticId { get; set; }

    /// <summary>
    /// Session de diagnostic associée.
    /// </summary>
    public SessionDiagnostic SessionDiagnostic { get; set; } = null!;

    /// <summary>
    /// Référence normalisée du dossier (ex. "REP-2026-0042").
    /// </summary>
    public string ReferenceDossier { get; set; } = string.Empty;

    /// <summary>
    /// Date et heure de clôture du diagnostic.
    /// </summary>
    public DateTime DateDiagnostic { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Identifiant de la panne principale formellement confirmée ou traitée.
    /// </summary>
    public int? PanneIdentifieeId { get; set; }

    /// <summary>
    /// Panne identifiée comme cause racine.
    /// </summary>
    public Panne? PanneIdentifiee { get; set; }

    /// <summary>
    /// Liste des composants remplacés ou réparés (ex. "IC Retimer SN75DP159, Pâte thermique MX-4, Condensateur CMS 10uF").
    /// </summary>
    public string? ComposantsRepares { get; set; }

    /// <summary>
    /// Résultat et bilan fonctionnel suite aux tests de requalification post-réparation.
    /// </summary>
    public string? ResultatApresReparation { get; set; }

    /// <summary>
    /// Remarques finales et recommandations d'entretien pour le client.
    /// </summary>
    public string? NotesTechnicien { get; set; }
}
