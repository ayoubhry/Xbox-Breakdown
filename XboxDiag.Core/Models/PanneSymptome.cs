namespace XboxDiag.Core.Models;

/// <summary>
/// Table de liaison entre une Panne et un Symptôme, pondérée par un score de pertinence.
/// </summary>
public class PanneSymptome
{
    /// <summary>
    /// Identifiant de la panne associée (clé étrangère).
    /// </summary>
    public int PanneId { get; set; }

    /// <summary>
    /// Entité Panne liée.
    /// </summary>
    public Panne Panne { get; set; } = null!;

    /// <summary>
    /// Identifiant du symptôme associé (clé étrangère).
    /// </summary>
    public int SymptomeId { get; set; }

    /// <summary>
    /// Entité Symptôme liée.
    /// </summary>
    public Symptome Symptome { get; set; } = null!;

    /// <summary>
    /// Poids ou degré de corrélation du symptôme pour cette panne (échelle de 1 à 10).
    /// </summary>
    public int Poids { get; set; }
}
