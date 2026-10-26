namespace XboxDiag.Core.Models;

/// <summary>
/// Règle logique déterminant l'impact du résultat d'un test sur une hypothèse de panne.
/// </summary>
public class RegleDiagnostic
{
    /// <summary>
    /// Identifiant unique de la règle.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la panne ciblée par la règle.
    /// </summary>
    public int PanneId { get; set; }

    /// <summary>
    /// Entité Panne concernée.
    /// </summary>
    public Panne Panne { get; set; } = null!;

    /// <summary>
    /// Identifiant du test technique déclencheur.
    /// </summary>
    public int TestDiagnosticId { get; set; }

    /// <summary>
    /// Test de diagnostic associé.
    /// </summary>
    public TestDiagnostic Test { get; set; } = null!;

    /// <summary>
    /// Résultat du test déclenchant l'application de la règle.
    /// </summary>
    public ResultatTest ResultatAttendu { get; set; }

    /// <summary>
    /// Effet qualitatif sur l'hypothèse (Renforcer, Affaiblir, Eliminer, Confirmer).
    /// </summary>
    public EffetRegle Effet { get; set; }

    /// <summary>
    /// Variation quantitative du score de confiance (-100 à +100).
    /// </summary>
    public int DeltaConfiance { get; set; }

    /// <summary>
    /// Source technique ou documentaire (manuel Microsoft, schémas boardview, expérience atelier).
    /// </summary>
    public string? Source { get; set; }

    /// <summary>
    /// Commentaire explicatif sur le raisonnement métier de la règle.
    /// </summary>
    public string? Commentaire { get; set; }
}
