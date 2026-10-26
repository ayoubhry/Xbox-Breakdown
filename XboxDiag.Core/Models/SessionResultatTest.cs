namespace XboxDiag.Core.Models;

/// <summary>
/// Enregistrement du résultat d'un test appliqué au cours d'une session de diagnostic.
/// </summary>
public class SessionResultatTest
{
    /// <summary>
    /// Identifiant unique du résultat de test.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la session de diagnostic parente.
    /// </summary>
    public int SessionDiagnosticId { get; set; }

    /// <summary>
    /// Session de diagnostic parente.
    /// </summary>
    public SessionDiagnostic SessionDiagnostic { get; set; } = null!;

    /// <summary>
    /// Identifiant du test exécuté.
    /// </summary>
    public int TestDiagnosticId { get; set; }

    /// <summary>
    /// Test de diagnostic associé.
    /// </summary>
    public TestDiagnostic TestDiagnostic { get; set; } = null!;

    /// <summary>
    /// Résultat constaté (Conforme, NonConforme, Incertain, Impossible).
    /// </summary>
    public ResultatTest Resultat { get; set; }

    /// <summary>
    /// Horodatage de l'exécution du test.
    /// </summary>
    public DateTime Horodatage { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Notes, mesures ou observations particulières du technicien.
    /// </summary>
    public string? NotesTechnicien { get; set; }
}
