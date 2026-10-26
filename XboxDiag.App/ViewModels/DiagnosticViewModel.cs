using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using XboxDiag.App.Models;
using XboxDiag.App.Services;
using XboxDiag.Core.Data;
using XboxDiag.Core.Engine;
using XboxDiag.Core.Models;

namespace XboxDiag.App.ViewModels;

/// <summary>
/// ViewModel principal de l'écran de diagnostic interactif guidé par le moteur expert.
/// </summary>
public class DiagnosticViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly IDiagnosticEngine _diagnosticEngine;
    private readonly DiagDbContext _dbContext;

    private int _sessionId;
    private SessionDiagnostic? _session;
    private TestDiagnostic? _prochainTest;
    private SessionHypothese? _selectedHypothese;
    private bool _isAjouterSymptomeOpen;
    private Symptome? _selectedSymptomeToAdd;

    public int SessionId
    {
        get => _sessionId;
        private set => SetProperty(ref _sessionId, value);
    }

    public SessionDiagnostic? Session
    {
        get => _session;
        private set
        {
            if (SetProperty(ref _session, value))
            {
                OnPropertyChanged(nameof(NomModele));
                OnPropertyChanged(nameof(NumeroSerie));
                OnPropertyChanged(nameof(DateCreationTexte));
                OnPropertyChanged(nameof(StatutTexte));
            }
        }
    }

    public TestDiagnostic? ProchainTest
    {
        get => _prochainTest;
        private set
        {
            if (SetProperty(ref _prochainTest, value))
            {
                OnPropertyChanged(nameof(HasProchainTest));
                OnPropertyChanged(nameof(ProchainTestEtoiles));
                (LancerTestCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool HasProchainTest => ProchainTest != null;

    public string ProchainTestEtoiles => ProchainTest?.Difficulte switch
    {
        NiveauDifficulte.Debutant => "⭐ Débutant (Sans démontage)",
        NiveauDifficulte.Intermediaire => "⭐⭐ Intermédiaire (Démontage)",
        NiveauDifficulte.Expert => "⭐⭐⭐ Expert (Mesures / CMS)",
        _ => ""
    };

    private List<SessionHypothese> _toutesLesHypotheses = new();
    private string _filtreHypotheses = "Toutes";

    public string FiltreHypotheses
    {
        get => _filtreHypotheses;
        set
        {
            if (SetProperty(ref _filtreHypotheses, value))
            {
                OnPropertyChanged(nameof(IsFiltreToutes));
                OnPropertyChanged(nameof(IsFiltreEnJeu));
                OnPropertyChanged(nameof(IsFiltreEcartees));
                AppliquerFiltreHypotheses();
            }
        }
    }

    public bool IsFiltreToutes => FiltreHypotheses == "Toutes";
    public bool IsFiltreEnJeu => FiltreHypotheses == "EnJeu";
    public bool IsFiltreEcartees => FiltreHypotheses == "Ecartees";

    public int NbHypothesesToutes => _toutesLesHypotheses.Count;
    public int NbHypothesesEnJeu => _toutesLesHypotheses.Count(h => h.Statut != StatutHypothese.EcarteeParTest);
    public int NbHypothesesEcartees => _toutesLesHypotheses.Count(h => h.Statut == StatutHypothese.EcarteeParTest);

    public SessionHypothese? SelectedHypothese
    {
        get => _selectedHypothese;
        set => SetProperty(ref _selectedHypothese, value);
    }

    public bool IsAjouterSymptomeOpen
    {
        get => _isAjouterSymptomeOpen;
        set => SetProperty(ref _isAjouterSymptomeOpen, value);
    }

    public Symptome? SelectedSymptomeToAdd
    {
        get => _selectedSymptomeToAdd;
        set
        {
            if (SetProperty(ref _selectedSymptomeToAdd, value))
            {
                (ValiderAjoutSymptomeCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string NomModele => Session?.ModeleConsole?.Nom ?? "Console Xbox";
    public string NumeroSerie => string.IsNullOrWhiteSpace(Session?.NumeroSerie) ? "Non renseigné" : Session.NumeroSerie;
    public string DateCreationTexte => Session?.DateCreation.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? "-";
    public string StatutTexte => Session?.Statut.ToString() ?? "-";

    public ObservableCollection<SessionHypothese> Hypotheses { get; } = new();
    public ObservableCollection<SessionResultatTest> ResultatsTests { get; } = new();
    public ObservableCollection<Symptome> SymptomesObserves { get; } = new();
    public ObservableCollection<Symptome> SymptomesDisponibles { get; } = new();

    public ICommand LancerTestCommand { get; }
    public ICommand FiltrerToutesCommand { get; }
    public ICommand FiltrerEnJeuCommand { get; }
    public ICommand FiltrerEcarteesCommand { get; }
    public ICommand OuvrirAjoutSymptomeCommand { get; }
    public ICommand ValiderAjoutSymptomeCommand { get; }
    public ICommand AnnulerAjoutSymptomeCommand { get; }
    public ICommand AnnulerDernierTestCommand { get; }
    public ICommand RecommencerCommand { get; }
    public ICommand GenererRapportCommand { get; }
    public ICommand TerminerDiagnosticCommand { get; }
    public ICommand RetourAccueilCommand { get; }

    public DiagnosticViewModel(NavigationService navigationService, IDiagnosticEngine diagnosticEngine, DiagDbContext dbContext)
    {
        _navigationService = navigationService;
        _diagnosticEngine = diagnosticEngine;
        _dbContext = dbContext;

        LancerTestCommand = new RelayCommand(LancerTest, () => HasProchainTest);
        FiltrerToutesCommand = new RelayCommand(() => FiltreHypotheses = "Toutes");
        FiltrerEnJeuCommand = new RelayCommand(() => FiltreHypotheses = "EnJeu");
        FiltrerEcarteesCommand = new RelayCommand(() => FiltreHypotheses = "Ecartees");
        OuvrirAjoutSymptomeCommand = new RelayCommand(OuvrirAjoutSymptome);
        ValiderAjoutSymptomeCommand = new RelayCommand(ValiderAjoutSymptome, () => SelectedSymptomeToAdd != null);
        AnnulerAjoutSymptomeCommand = new RelayCommand(AnnulerAjoutSymptome);
        AnnulerDernierTestCommand = new RelayCommand(AnnulerDernierTest, () => ResultatsTests.Count > 0);
        RecommencerCommand = new RelayCommand(Recommencer);
        GenererRapportCommand = new RelayCommand(GenererRapport);
        TerminerDiagnosticCommand = new RelayCommand(TerminerDiagnostic);
        RetourAccueilCommand = new RelayCommand(RetourAccueil);
    }

    public void Initialiser(object? parameter)
    {
        if (parameter is int sid)
        {
            SessionId = sid;
            Rafraichir();
        }
    }

    public void Rafraichir()
    {
        Session = _diagnosticEngine.ReprendreSession(SessionId);
        if (Session == null)
            return;

        // 1. Hypothèses triées par confiance décroissante
        _toutesLesHypotheses = _diagnosticEngine.ObtenirHypotheses(SessionId);
        OnPropertyChanged(nameof(NbHypothesesToutes));
        OnPropertyChanged(nameof(NbHypothesesEnJeu));
        OnPropertyChanged(nameof(NbHypothesesEcartees));
        AppliquerFiltreHypotheses();

        // 2. Prochain test recommandé
        ProchainTest = _diagnosticEngine.ObtenirProchainTest(SessionId);

        // 3. Tests complétés ordonnés chronologiquement inverse
        ResultatsTests.Clear();
        foreach (var res in Session.ResultatsTests.OrderByDescending(r => r.Horodatage))
        {
            ResultatsTests.Add(res);
        }

        // 4. Symptômes observés
        SymptomesObserves.Clear();
        foreach (var ss in Session.SessionSymptomes)
        {
            if (ss.Symptome != null)
            {
                SymptomesObserves.Add(ss.Symptome);
            }
        }

        // Mettre à jour l'état des commandes
        (AnnulerDernierTestCommand as RelayCommand)?.RaiseCanExecuteChanged();
        (LancerTestCommand as RelayCommand)?.RaiseCanExecuteChanged();

        IsAjouterSymptomeOpen = false;
        SelectedSymptomeToAdd = null;
    }

    private void AppliquerFiltreHypotheses()
    {
        Hypotheses.Clear();
        var query = _toutesLesHypotheses.AsEnumerable();
        if (FiltreHypotheses == "EnJeu")
        {
            query = query.Where(h => h.Statut != StatutHypothese.EcarteeParTest);
        }
        else if (FiltreHypotheses == "Ecartees")
        {
            query = query.Where(h => h.Statut == StatutHypothese.EcarteeParTest);
        }

        foreach (var h in query)
        {
            Hypotheses.Add(h);
        }

        if (SelectedHypothese == null || !Hypotheses.Contains(SelectedHypothese))
        {
            SelectedHypothese = Hypotheses.FirstOrDefault();
        }
    }

    private void LancerTest()
    {
        if (ProchainTest == null)
            return;

        var param = new AssistantTestParam
        {
            SessionId = SessionId,
            TestDiagnosticId = ProchainTest.Id
        };

        _navigationService.NaviguerVers(NavigationPage.AssistantTest, param);
    }

    private void OuvrirAjoutSymptome()
    {
        if (Session == null)
            return;

        var idsDejaAjoutes = SymptomesObserves.Select(s => s.Id).ToHashSet();

        SymptomesDisponibles.Clear();
        var dispo = _dbContext.Symptomes
            .Where(s => s.Actif &&
                        (s.ModeleConsoleId == null || s.ModeleConsoleId == Session.ModeleConsoleId) &&
                        !idsDejaAjoutes.Contains(s.Id))
            .OrderBy(s => s.Categorie)
            .ThenBy(s => s.Ordre)
            .ToList();

        foreach (var item in dispo)
        {
            SymptomesDisponibles.Add(item);
        }

        SelectedSymptomeToAdd = SymptomesDisponibles.FirstOrDefault();
        IsAjouterSymptomeOpen = true;
    }

    private void ValiderAjoutSymptome()
    {
        if (SelectedSymptomeToAdd == null)
            return;

        _diagnosticEngine.AjouterSymptome(SessionId, SelectedSymptomeToAdd.Id);
        Rafraichir();
    }

    private void AnnulerAjoutSymptome()
    {
        IsAjouterSymptomeOpen = false;
        SelectedSymptomeToAdd = null;
    }

    private void AnnulerDernierTest()
    {
        if (ResultatsTests.Count == 0)
            return;

        var result = MessageBox.Show(
            "Voulez-vous vraiment annuler le dernier test enregistré ? L'état des hypothèses sera recalculé.",
            "Confirmation d'annulation",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            _diagnosticEngine.AnnulerDernierTest(SessionId);
            Rafraichir();
        }
    }

    private void Recommencer()
    {
        var result = MessageBox.Show(
            "Êtes-vous sûr de vouloir réinitialiser l'ensemble des tests pour cette session ? Toutes les mesures saisies seront effacées.",
            "Réinitialisation du diagnostic",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            _diagnosticEngine.RecommencerDiagnostic(SessionId);
            Rafraichir();
        }
    }

    private void GenererRapport()
    {
        _navigationService.NaviguerVers(NavigationPage.Rapport, SessionId);
    }

    private void TerminerDiagnostic()
    {
        _navigationService.NaviguerVers(NavigationPage.Rapport, SessionId);
    }

    private void RetourAccueil()
    {
        _navigationService.NaviguerVers(NavigationPage.Accueil);
    }
}
