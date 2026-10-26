using System;

namespace XboxDiag.App.Services;

/// <summary>
/// Pages navigables de l'application XboxDiag.
/// </summary>
public enum NavigationPage
{
    Accueil,
    IdentificationConsole,
    SelectionSymptomes,
    Diagnostic,
    AssistantTest,
    Rapport,
    Historique,
    BaseConnaissances
}

/// <summary>
/// Service gérant la navigation entre les différents écrans de l'application.
/// </summary>
public class NavigationService
{
    /// <summary>
    /// Déclenché lorsqu'une navigation vers une nouvelle page est requise.
    /// </summary>
    public event Action<NavigationPage, object?>? PageChangee;

    /// <summary>
    /// Page actuellement affichée.
    /// </summary>
    public NavigationPage PageActuelle { get; private set; } = NavigationPage.Accueil;

    /// <summary>
    /// Paramètre optionnel transmis à la page courante.
    /// </summary>
    public object? ParametreActuel { get; private set; }

    /// <summary>
    /// Navigue vers la page cible spécifiée avec un paramètre optionnel.
    /// </summary>
    /// <param name="page">La page de destination.</param>
    /// <param name="parameter">Paramètre de contexte (ID de session, IDs de tests, etc.).</param>
    public void NaviguerVers(NavigationPage page, object? parameter = null)
    {
        PageActuelle = page;
        ParametreActuel = parameter;
        PageChangee?.Invoke(page, parameter);
    }
}
