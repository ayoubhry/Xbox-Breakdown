using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using XboxDiag.App.Services;
using XboxDiag.Core.Data;
using XboxDiag.Core.Models;

namespace XboxDiag.App.ViewModels;

/// <summary>
/// ViewModel permettant de consulter librement l'arbre des connaissances expertes (Pannes, Tests, Symptômes).
/// </summary>
public class BaseConnaissancesViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly DiagDbContext _dbContext;

    private Panne? _selectedPanne;
    private TestDiagnostic? _selectedTest;
    private int _selectedTabIndex = 0;
    private string _rechercheTexte = string.Empty;

    private List<Panne> _toutesLesPannes = new();
    private List<TestDiagnostic> _tousLesTests = new();
    private List<Symptome> _tousLesSymptomes = new();

    public ObservableCollection<Panne> Pannes { get; } = new();
    public ObservableCollection<TestDiagnostic> Tests { get; } = new();
    public ObservableCollection<Symptome> Symptomes { get; } = new();

    public string RechercheTexte
    {
        get => _rechercheTexte;
        set
        {
            if (SetProperty(ref _rechercheTexte, value))
            {
                FiltrerDonnees();
            }
        }
    }

    public Panne? SelectedPanne
    {
        get => _selectedPanne;
        set => SetProperty(ref _selectedPanne, value);
    }

    public TestDiagnostic? SelectedTest
    {
        get => _selectedTest;
        set => SetProperty(ref _selectedTest, value);
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set => SetProperty(ref _selectedTabIndex, value);
    }

    public string TitreOngletPannes => $"🔧 Pannes ({Pannes.Count})";
    public string TitreOngletTests => $"🧪 Fiches Tests ({Tests.Count})";
    public string TitreOngletSymptomes => $"📋 Catalogue ({Symptomes.Count})";

    public ICommand RetourCommand { get; }
    public ICommand EffacerRechercheCommand { get; }

    public BaseConnaissancesViewModel(NavigationService navigationService, DiagDbContext dbContext)
    {
        _navigationService = navigationService;
        _dbContext = dbContext;

        RetourCommand = new RelayCommand(Retour);
        EffacerRechercheCommand = new RelayCommand(() => RechercheTexte = string.Empty);
    }

    public void ChargerDonnees()
    {
        _toutesLesPannes = _dbContext.Pannes
            .Include(p => p.PanneSymptomes)
                .ThenInclude(ps => ps.Symptome)
            .Include(p => p.Regles)
                .ThenInclude(r => r.Test)
            .ToList();

        _tousLesTests = _dbContext.Tests
            .Include(t => t.Regles)
                .ThenInclude(r => r.Panne)
            .OrderBy(t => (int)t.Difficulte)
            .ThenBy(t => t.Id)
            .ToList();

        _tousLesSymptomes = _dbContext.Symptomes
            .Include(s => s.ModeleConsole)
            .OrderBy(s => s.Categorie)
            .ThenBy(s => s.Ordre)
            .ToList();

        FiltrerDonnees();
    }

    private void FiltrerDonnees()
    {
        string q = (_rechercheTexte ?? "").Trim().ToLowerInvariant();

        // 1. Pannes
        Pannes.Clear();
        foreach (var p in _toutesLesPannes)
        {
            if (string.IsNullOrWhiteSpace(q) ||
                p.Nom.ToLowerInvariant().Contains(q) ||
                (p.Description != null && p.Description.ToLowerInvariant().Contains(q)))
            {
                Pannes.Add(p);
            }
        }
        if (SelectedPanne == null || !Pannes.Contains(SelectedPanne))
        {
            SelectedPanne = Pannes.FirstOrDefault();
        }

        // 2. Tests
        Tests.Clear();
        foreach (var t in _tousLesTests)
        {
            if (string.IsNullOrWhiteSpace(q) ||
                t.Nom.ToLowerInvariant().Contains(q) ||
                t.Objectif.ToLowerInvariant().Contains(q) ||
                (t.PourquoiUtile != null && t.PourquoiUtile.ToLowerInvariant().Contains(q)) ||
                (t.MaterielNecessaire != null && t.MaterielNecessaire.ToLowerInvariant().Contains(q)))
            {
                Tests.Add(t);
            }
        }
        if (SelectedTest == null || !Tests.Contains(SelectedTest))
        {
            SelectedTest = Tests.FirstOrDefault();
        }

        // 3. Symptômes
        Symptomes.Clear();
        foreach (var s in _tousLesSymptomes)
        {
            if (string.IsNullOrWhiteSpace(q) ||
                s.Description.ToLowerInvariant().Contains(q) ||
                s.Categorie.ToString().ToLowerInvariant().Contains(q))
            {
                Symptomes.Add(s);
            }
        }

        OnPropertyChanged(nameof(TitreOngletPannes));
        OnPropertyChanged(nameof(TitreOngletTests));
        OnPropertyChanged(nameof(TitreOngletSymptomes));
    }

    private void Retour()
    {
        _navigationService.NaviguerVers(NavigationPage.Accueil);
    }
}
