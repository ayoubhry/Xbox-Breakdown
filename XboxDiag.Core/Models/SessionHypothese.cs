namespace XboxDiag.Core.Models;

/// <summary>
/// Évaluation dynamique d'une hypothèse de panne au sein d'une session de diagnostic.
/// </summary>
public class SessionHypothese
{
    /// <summary>
    /// Identifiant unique de l'hypothèse de session.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la session de diagnostic.
    /// </summary>
    public int SessionDiagnosticId { get; set; }

    /// <summary>
    /// Session de diagnostic associée.
    /// </summary>
    public SessionDiagnostic SessionDiagnostic { get; set; } = null!;

    /// <summary>
    /// Identifiant de la panne ciblée.
    /// </summary>
    public int PanneId { get; set; }

    /// <summary>
    /// Panne évaluée.
    /// </summary>
    public Panne Panne { get; set; } = null!;

    /// <summary>
    /// Statut actuel de l'hypothèse (CompatibleSymptomes, RenforceeParTests, EcarteeParTest, ConfirmeeParVerification, etc.).
    /// </summary>
    public StatutHypothese Statut { get; set; }

    /// <summary>
    /// Indice de confiance calculé (pourcentage de 0 à 100).
    /// </summary>
    public int Confiance { get; set; }

    /// <summary>
    /// Justification textuelle détaillée du calcul et des déductions logiques.
    /// </summary>
    public string Justification { get; set; } = string.Empty;
}
