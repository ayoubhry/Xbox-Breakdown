namespace XboxDiag.App.Models;

/// <summary>
/// Données d'identification de la console saisies par l'utilisateur.
/// </summary>
public class IdentificationConsoleParam
{
    public int ModeleConsoleId { get; set; }
    public string NomModele { get; set; } = string.Empty;
    public string? NumeroSerie { get; set; }
    public string? EtatGeneral { get; set; }
    public string? ReparationsPrecedentes { get; set; }
}

/// <summary>
/// Paramètres requis pour lancer un test d'assistance.
/// </summary>
public class AssistantTestParam
{
    public int SessionId { get; set; }
    public int TestDiagnosticId { get; set; }
}
