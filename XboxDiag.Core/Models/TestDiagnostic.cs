namespace XboxDiag.Core.Models;

/// <summary>
/// Représente une procédure technique de test pour diagnostiquer une console Xbox.
/// </summary>
public class TestDiagnostic
{
    /// <summary>
    /// Identifiant unique du test.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom usuel du test technique.
    /// </summary>
    public string Nom { get; set; } = string.Empty;

    /// <summary>
    /// Objectif mesurable ou observable du test.
    /// </summary>
    public string Objectif { get; set; } = string.Empty;

    /// <summary>
    /// Explication de la pertinence de ce test dans la démarche diagnostique.
    /// </summary>
    public string PourquoiUtile { get; set; } = string.Empty;

    /// <summary>
    /// Outillage et matériel indispensables (ex. multimètre, microscope, câble HDMI neuf).
    /// </summary>
    public string? MaterielNecessaire { get; set; }

    /// <summary>
    /// Étapes pas à pas pour réaliser le test de manière sécurisée et rigoureuse.
    /// </summary>
    public string Etapes { get; set; } = string.Empty;

    /// <summary>
    /// Niveau de compétence requis pour effectuer l'opération.
    /// </summary>
    public NiveauDifficulte Difficulte { get; set; }

    /// <summary>
    /// Risques potentiels pour le technicien ou pour la console (court-circuit, ESD, brûlure).
    /// </summary>
    public string? Risques { get; set; }

    /// <summary>
    /// Consignes de sécurité et précautions à respecter impérativement.
    /// </summary>
    public string? PrecautionsSecurite { get; set; }

    /// <summary>
    /// Identifiant du modèle spécifique si le test est réservé à une déclinaison.
    /// </summary>
    public int? ModeleConsoleId { get; set; }

    /// <summary>
    /// Modèle de console associé le cas échéant.
    /// </summary>
    public ModeleConsole? ModeleConsole { get; set; }

    /// <summary>
    /// Indique si le test est actif et éligible lors des sessions.
    /// </summary>
    public bool Actif { get; set; } = true;

    /// <summary>
    /// Règles d'inférence associées à ce test pour déduire ou infirmer des pannes.
    /// </summary>
    public List<RegleDiagnostic> Regles { get; set; } = new();

    /// <summary>
    /// Résultats enregistrés pour ce test lors des différentes sessions.
    /// </summary>
    public List<SessionResultatTest> SessionResultats { get; set; } = new();
}
