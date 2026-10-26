namespace XboxDiag.Core.Models;

/// <summary>
/// Table de liaison entre une Session de diagnostic et les Symptômes signalés.
/// </summary>
public class SessionSymptome
{
    /// <summary>
    /// Identifiant de la session de diagnostic.
    /// </summary>
    public int SessionDiagnosticId { get; set; }

    /// <summary>
    /// Session de diagnostic associée.
    /// </summary>
    public SessionDiagnostic SessionDiagnostic { get; set; } = null!;

    /// <summary>
    /// Identifiant du symptôme constaté.
    /// </summary>
    public int SymptomeId { get; set; }

    /// <summary>
    /// Symptôme associé.
    /// </summary>
    public Symptome Symptome { get; set; } = null!;
}
