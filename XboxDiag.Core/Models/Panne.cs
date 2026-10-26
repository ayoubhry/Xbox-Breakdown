namespace XboxDiag.Core.Models;

/// <summary>
/// Représente une panne matérielle ou logicielle répertoriée dans la base de connaissances.
/// </summary>
public class Panne
{
    /// <summary>
    /// Identifiant unique de la panne.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom technique de la panne (ex. "Circuit HDMI défectueux (Retimer/TDP158)").
    /// </summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Description détaillée de la défaillance, de sa cause et de son impact.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Modèle spécifique auquel s'applique la panne, ou null si commun.
    /// </summary>
    public int? ModeleConsoleId { get; set; }

    /// <summary>
    /// Référence vers le modèle associé.
    /// </summary>
    public ModeleConsole? ModeleConsole { get; set; }

    /// <summary>
    /// Indique si la panne est active dans l'arbre de décision.
    /// </summary>
    public bool Actif { get; set; } = true;

    /// <summary>
    /// Liste des liaisons entre cette panne et les symptômes observés.
    /// </summary>
    public List<PanneSymptome> PanneSymptomes { get; set; } = new();

    /// <summary>
    /// Règles de diagnostic associées à cette panne.
    /// </summary>
    public List<RegleDiagnostic> Regles { get; set; } = new();

    /// <summary>
    /// Hypothèses émises dans les différentes sessions de diagnostic.
    /// </summary>
    public List<SessionHypothese> Hypotheses { get; set; } = new();

    /// <summary>
    /// Dossiers de réparation ayant confirmé cette panne comme cause résolue.
    /// </summary>
    public List<DossierReparation> DossiersReparation { get; set; } = new();
}
