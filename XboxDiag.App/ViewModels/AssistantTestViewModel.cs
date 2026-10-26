using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using XboxDiag.App.Models;
using XboxDiag.App.Services;
using XboxDiag.Core.Data;
using XboxDiag.Core.Engine;
using XboxDiag.Core.Models;

namespace XboxDiag.App.ViewModels;

/// <summary>
/// Étape numérotée individuelle d'une procédure technique de test.
/// </summary>
public class EtapeTestItem
{
    public int Numero { get; set; }
    public string Description { get; set; } = string.Empty;
}

/// <summary>
/// ViewModel guidant pas à pas le technicien lors de la réalisation d'un test technique.
/// </summary>
public class AssistantTestViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly IDiagnosticEngine _diagnosticEngine;
    private readonly DiagDbContext _dbContext;

    private int _sessionId;
    private int _testId;
    private TestDiagnostic? _test;
    private string? _notesTechnicien;

    public int SessionId
    {
        get => _sessionId;
        private set => SetProperty(ref _sessionId, value);
    }

    public ObservableCollection<EtapeTestItem> ListeEtapes { get; } = new();

    public TestDiagnostic? Test
    {
        get => _test;
        private set
        {
            if (SetProperty(ref _test, value))
            {
                OnPropertyChanged(nameof(Nom));
                OnPropertyChanged(nameof(Objectif));
                OnPropertyChanged(nameof(PourquoiUtile));
                OnPropertyChanged(nameof(MaterielNecessaire));
                OnPropertyChanged(nameof(Etapes));
                OnPropertyChanged(nameof(DifficulteTexte));
                OnPropertyChanged(nameof(Risques));
                OnPropertyChanged(nameof(PrecautionsSecurite));
                OnPropertyChanged(nameof(HasPrecautions));
                OnPropertyChanged(nameof(HasRisques));
                MettreAJourListeEtapes();
            }
        }
    }

    public string Nom => Test?.Nom ?? "Test technique";
    public string Objectif => Test?.Objectif ?? string.Empty;
    public string PourquoiUtile => Test?.PourquoiUtile ?? string.Empty;
    public string MaterielNecessaire => string.IsNullOrWhiteSpace(Test?.MaterielNecessaire) ? "Aucun matériel spécifique requis." : Test.MaterielNecessaire;
    public string Etapes => Test?.Etapes ?? string.Empty;
    public string DifficulteTexte => Test != null ? ObtenirLibelleDifficulte(Test.Difficulte) : "-";
    public string Risques => Test?.Risques ?? string.Empty;
    public string PrecautionsSecurite => Test?.PrecautionsSecurite ?? string.Empty;
    public bool HasPrecautions => !string.IsNullOrWhiteSpace(PrecautionsSecurite);
    public bool HasRisques => !string.IsNullOrWhiteSpace(Risques);

    public string? NotesTechnicien
    {
        get => _notesTechnicien;
        set => SetProperty(ref _notesTechnicien, value);
    }

    public ICommand ConformeCommand { get; }
    public ICommand NonConformeCommand { get; }
    public ICommand IncertainCommand { get; }
    public ICommand ImpossibleCommand { get; }
    public ICommand RetourCommand { get; }

    public AssistantTestViewModel(NavigationService navigationService, IDiagnosticEngine diagnosticEngine, DiagDbContext dbContext)
    {
        _navigationService = navigationService;
        _diagnosticEngine = diagnosticEngine;
        _dbContext = dbContext;

        ConformeCommand = new RelayCommand(() => EnregistrerResultat(ResultatTest.Conforme));
        NonConformeCommand = new RelayCommand(() => EnregistrerResultat(ResultatTest.NonConforme));
        IncertainCommand = new RelayCommand(() => EnregistrerResultat(ResultatTest.Incertain));
        ImpossibleCommand = new RelayCommand(() => EnregistrerResultat(ResultatTest.Impossible));
        RetourCommand = new RelayCommand(Retour);
    }

    public void Initialiser(object? parameter)
    {
        if (parameter is AssistantTestParam param)
        {
            SessionId = param.SessionId;
            _testId = param.TestDiagnosticId;
            NotesTechnicien = null;

            Test = _dbContext.Tests.Find(_testId);
        }
    }

    private void EnregistrerResultat(ResultatTest resultat)
    {
        _diagnosticEngine.EnregistrerResultatTest(SessionId, _testId, resultat, NotesTechnicien?.Trim());
        _navigationService.NaviguerVers(NavigationPage.Diagnostic, SessionId);
    }

    private void Retour()
    {
        _navigationService.NaviguerVers(NavigationPage.Diagnostic, SessionId);
    }

    private void MettreAJourListeEtapes()
    {
        ListeEtapes.Clear();
        if (string.IsNullOrWhiteSpace(Test?.Etapes))
            return;

        var lignes = Test.Etapes.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        int indexAuto = 1;

        foreach (var brute in lignes)
        {
            string texte = brute.Trim();
            if (string.IsNullOrWhiteSpace(texte))
                continue;

            int num = indexAuto;
            int pointIdx = texte.IndexOf('.');
            if (pointIdx > 0 && pointIdx <= 3 && int.TryParse(texte.Substring(0, pointIdx), out int parsedNum))
            {
                num = parsedNum;
                texte = texte.Substring(pointIdx + 1).Trim();
            }

            ListeEtapes.Add(new EtapeTestItem
            {
                Numero = num,
                Description = texte
            });

            indexAuto++;
        }
    }

    private static string ObtenirLibelleDifficulte(NiveauDifficulte difficulte)
    {
        return difficulte switch
        {
            NiveauDifficulte.Debutant => "Niveau 1 — Débutant (Sans démontage)",
            NiveauDifficulte.Intermediaire => "Niveau 2 — Intermédiaire (Démontage / Multimètre)",
            NiveauDifficulte.Expert => "Niveau 3 — Expert (Micro-électronique / CMS / Oscilloscope)",
            _ => difficulte.ToString()
        };
    }
}
