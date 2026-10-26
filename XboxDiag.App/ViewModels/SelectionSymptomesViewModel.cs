using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using XboxDiag.App.Models;
using XboxDiag.App.Services;
using XboxDiag.Core.Data;
using XboxDiag.Core.Engine;
using XboxDiag.Core.Models;

namespace XboxDiag.App.ViewModels;

/// <summary>
/// Élément représentant un symptôme sélectionnable avec liaison bidirectionnelle.
/// </summary>
public class SymptomeItemViewModel : BaseViewModel
{
    private bool _isSelected;
    private bool _isVisible = true;

    public Symptome Symptome { get; }
    public int Id => Symptome.Id;
    public string Description => Symptome.Description;
    public CategorieSymptome Categorie => Symptome.Categorie;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (SetProperty(ref _isSelected, value))
            {
                OnSelectionChanged?.Invoke();
            }
        }
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => SetProperty(ref _isVisible, value);
    }

    public Action? OnSelectionChanged { get; set; }

    public SymptomeItemViewModel(Symptome symptome)
    {
        Symptome = symptome;
    }
}

/// <summary>
/// Groupe de symptômes appartenant à une même catégorie fonctionnelle avec décompte réactif.
/// </summary>
public class CategorieSymptomesGroupViewModel : BaseViewModel
{
    public CategorieSymptome Categorie { get; }
    public string Titre { get; }
    public ObservableCollection<SymptomeItemViewModel> Symptomes { get; } = new();

    public int NbSelectionnes => Symptomes.Count(s => s.IsSelected);
    public bool HasSelectionnes => NbSelectionnes > 0;
    public int NbTotal => Symptomes.Count;

    public void NotifierChangementSelection()
    {
        OnPropertyChanged(nameof(NbSelectionnes));
        OnPropertyChanged(nameof(HasSelectionnes));
    }

    public CategorieSymptomesGroupViewModel(CategorieSymptome categorie, string titre, IEnumerable<SymptomeItemViewModel> symptomes)
    {
        Categorie = categorie;
        Titre = titre;
        foreach (var s in symptomes)
        {
            Symptomes.Add(s);
        }
    }
}

/// <summary>
/// ViewModel gérant le choix initial des symptômes constatés avant démarrage du diagnostic.
/// </summary>
public class SelectionSymptomesViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly IDiagnosticEngine _diagnosticEngine;
    private readonly DiagDbContext _dbContext;

    private IdentificationConsoleParam? _consoleParam;
    private int _modeleConsoleId;
    private string _nomModele = "Xbox";
    private string _texteRecherche = string.Empty;
    private int _nbSymptomesSelectionnes;

    public ObservableCollection<CategorieSymptomesGroupViewModel> GroupesCategories { get; } = new();
    public List<SymptomeItemViewModel> TousLesSymptomes { get; } = new();

    public string NomModele
    {
        get => _nomModele;
        set => SetProperty(ref _nomModele, value);
    }

    public string TexteRecherche
    {
        get => _texteRecherche;
        set
        {
            if (SetProperty(ref _texteRecherche, value))
            {
                FiltrerSymptomes();
            }
        }
    }

    public int NbSymptomesSelectionnes
    {
        get => _nbSymptomesSelectionnes;
        private set
        {
            if (SetProperty(ref _nbSymptomesSelectionnes, value))
            {
                OnPropertyChanged(nameof(HasNoSelection));
                OnPropertyChanged(nameof(HasSelection));
            }
        }
    }

    public bool HasNoSelection => NbSymptomesSelectionnes == 0;
    public bool HasSelection => NbSymptomesSelectionnes > 0;

    public ICommand DemarrerDiagnosticCommand { get; }
    public ICommand RetourCommand { get; }
    public ICommand EffacerSelectionCommand { get; }

    public SelectionSymptomesViewModel(NavigationService navigationService, IDiagnosticEngine diagnosticEngine, DiagDbContext dbContext)
    {
        _navigationService = navigationService;
        _diagnosticEngine = diagnosticEngine;
        _dbContext = dbContext;

        DemarrerDiagnosticCommand = new RelayCommand(DemarrerDiagnostic, CanDemarrerDiagnostic);
        RetourCommand = new RelayCommand(Retour);
        EffacerSelectionCommand = new RelayCommand(EffacerSelection);
    }

    public void Initialiser(object? parameter)
    {
        if (parameter is IdentificationConsoleParam cp)
        {
            _consoleParam = cp;
            _modeleConsoleId = cp.ModeleConsoleId;
        }
        else if (parameter is int id)
        {
            _modeleConsoleId = id;
            var mod = _dbContext.Modeles.Find(id);
            _consoleParam = new IdentificationConsoleParam
            {
                ModeleConsoleId = id,
                NomModele = mod?.Nom ?? "Xbox"
            };
        }
        else
        {
            var premierModele = _dbContext.Modeles.FirstOrDefault();
            _modeleConsoleId = premierModele?.Id ?? 1;
            _consoleParam = new IdentificationConsoleParam
            {
                ModeleConsoleId = _modeleConsoleId,
                NomModele = premierModele?.Nom ?? "Xbox"
            };
        }

        NomModele = _consoleParam?.NomModele ?? "Xbox";
        ChargerSymptomes();
    }

    private void ChargerSymptomes()
    {
        TousLesSymptomes.Clear();
        GroupesCategories.Clear();

        var symptomesDb = _dbContext.Symptomes
            .Where(s => s.Actif && (s.ModeleConsoleId == null || s.ModeleConsoleId == _modeleConsoleId))
            .OrderBy(s => s.Categorie)
            .ThenBy(s => s.Ordre)
            .ToList();

        foreach (var s in symptomesDb)
        {
            var item = new SymptomeItemViewModel(s)
            {
                OnSelectionChanged = ActualiserCompteurSelection
            };
            TousLesSymptomes.Add(item);
        }

        var categoriesGroupes = TousLesSymptomes
            .GroupBy(s => s.Categorie)
            .OrderBy(g => (int)g.Key);

        foreach (var group in categoriesGroupes)
        {
            string titreCategorie = ObtenirLibelleCategorie(group.Key);
            GroupesCategories.Add(new CategorieSymptomesGroupViewModel(group.Key, titreCategorie, group));
        }

        ActualiserCompteurSelection();
    }

    private void ActualiserCompteurSelection()
    {
        NbSymptomesSelectionnes = TousLesSymptomes.Count(s => s.IsSelected);
        foreach (var group in GroupesCategories)
        {
            group.NotifierChangementSelection();
        }
        (DemarrerDiagnosticCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void FiltrerSymptomes()
    {
        string recherche = _texteRecherche.Trim().ToLowerInvariant();
        foreach (var item in TousLesSymptomes)
        {
            item.IsVisible = string.IsNullOrWhiteSpace(recherche) ||
                             item.Description.ToLowerInvariant().Contains(recherche);
        }
    }

    private void EffacerSelection()
    {
        foreach (var item in TousLesSymptomes)
        {
            item.IsSelected = false;
        }
    }

    private bool CanDemarrerDiagnostic()
    {
        return NbSymptomesSelectionnes > 0;
    }

    private void DemarrerDiagnostic()
    {
        var selectedIds = TousLesSymptomes
            .Where(s => s.IsSelected)
            .Select(s => s.Id)
            .ToList();

        if (selectedIds.Count == 0)
            return;

        var session = _diagnosticEngine.DemarrerSession(_modeleConsoleId, selectedIds);

        // Mise à jour des informations complémentaires de la session si saisies
        if (_consoleParam != null)
        {
            var sessionDb = _dbContext.Sessions.Find(session.Id);
            if (sessionDb != null)
            {
                sessionDb.NumeroSerie = _consoleParam.NumeroSerie;
                sessionDb.EtatGeneral = _consoleParam.EtatGeneral;
                sessionDb.ReparationsPrecedentes = _consoleParam.ReparationsPrecedentes;
                _dbContext.SaveChanges();
            }
        }

        _navigationService.NaviguerVers(NavigationPage.Diagnostic, session.Id);
    }

    private void Retour()
    {
        _navigationService.NaviguerVers(NavigationPage.IdentificationConsole);
    }

    private static string ObtenirLibelleCategorie(CategorieSymptome categorie)
    {
        return categorie switch
        {
            CategorieSymptome.Alimentation => "⚡ Alimentation & Démarrage",
            CategorieSymptome.Affichage => "📺 Affichage & Signal Vidéo HDMI",
            CategorieSymptome.LecteurOptique => "💿 Lecteur Optique & Blu-ray",
            CategorieSymptome.Stockage => "💾 Stockage & Disque Dur (Erreurs Exx)",
            CategorieSymptome.Thermique => "❄️ Dissipation Thermique & Ventilateur",
            CategorieSymptome.Connectique => "📡 Connectique, Wi-Fi & Manettes",
            CategorieSymptome.PanneElectronique => "🔬 Électronique Carte Mère (Composants / Courts-circuits)",
            _ => categorie.ToString()
        };
    }
}
