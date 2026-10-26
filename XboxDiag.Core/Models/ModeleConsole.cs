namespace XboxDiag.Core.Models;

/// <summary>
/// Représente un modèle de console Xbox (ex. Xbox One Fat, One S, One X).
/// </summary>
public class ModeleConsole
{
    /// <summary>
    /// Identifiant unique du modèle de console.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Famille de la console.
    /// </summary>
    public ConsoleFamille Famille { get; set; }

    /// <summary>
    /// Nom usuel du modèle (ex. "Xbox One S").
    /// </summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Référence constructeur / numéro de carte mère (ex. "Model 1681", "Model 1787").
    /// </summary>
    public string? ReferenceMaterielle { get; set; }

    /// <summary>
    /// Remarques techniques spécifiques au modèle.
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Indique si le modèle est actuellement pris en charge dans l'outil.
    /// </summary>
    public bool Actif { get; set; } = true;

    /// <summary>
    /// Sessions de diagnostic associées à ce modèle.
    /// </summary>
    public List<SessionDiagnostic> Sessions { get; set; } = new();
}
