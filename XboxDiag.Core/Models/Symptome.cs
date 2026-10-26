namespace XboxDiag.Core.Models;

/// <summary>
/// Représente un symptôme ou dysfonctionnement observable sur une console Xbox.
/// </summary>
public class Symptome
{
    /// <summary>
    /// Identifiant unique du symptôme.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Catégorie fonctionnelle (Alimentation, Affichage, Stockage, etc.).
    /// </summary>
    public CategorieSymptome Categorie { get; set; }

    /// <summary>
    /// Description claire du problème constaté.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Identifiant du modèle spécifique si le symptôme ne concerne qu'un modèle particulier (null si commun).
    /// </summary>
    public int? ModeleConsoleId { get; set; }

    /// <summary>
    /// Référence vers le modèle spécifique le cas échéant.
    /// </summary>
    public ModeleConsole? ModeleConsole { get; set; }

    /// <summary>
    /// Indique si le symptôme est proposé dans l'interface de diagnostic.
    /// </summary>
    public bool Actif { get; set; } = true;

    /// <summary>
    /// Ordre d'affichage dans la liste pour l'ergonomie.
    /// </summary>
    public int Ordre { get; set; } = 0;

    /// <summary>
    /// Pannes potentielles liées à ce symptôme.
    /// </summary>
    public List<PanneSymptome> PanneSymptomes { get; set; } = new();

    /// <summary>
    /// Sessions de diagnostic ayant signalé ce symptôme.
    /// </summary>
    public List<SessionSymptome> SessionSymptomes { get; set; } = new();
}
