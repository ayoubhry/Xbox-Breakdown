using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Extensions.DependencyInjection;
using XboxDiag.App.Services;
using XboxDiag.App.Views;

namespace XboxDiag.App.ViewModels;

/// <summary>
/// ViewModel principal coordonnant l'affichage des vues et le tableau de bord Xbox global.
/// </summary>
public class MainViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly IServiceProvider _serviceProvider;
    private readonly DispatcherTimer _timerHorloge;

    private UserControl? _currentView;
    private NavigationPage _activePage = NavigationPage.Accueil;
    private string _heureActuelle = "";
    private string _dateActuelle = "";

    public UserControl? CurrentView
    {
        get => _currentView;
        set => SetProperty(ref _currentView, value);
    }

    public NavigationPage ActivePage
    {
        get => _activePage;
        set
        {
            if (SetProperty(ref _activePage, value))
            {
                OnPropertyChanged(nameof(IsAccueilActive));
                OnPropertyChanged(nameof(IsNouveauDiagnosticActive));
                OnPropertyChanged(nameof(IsHistoriqueActive));
                OnPropertyChanged(nameof(IsBaseConnaissancesActive));
            }
        }
    }

    public bool IsAccueilActive => ActivePage == NavigationPage.Accueil;
    public bool IsNouveauDiagnosticActive => ActivePage == NavigationPage.IdentificationConsole ||
                                             ActivePage == NavigationPage.SelectionSymptomes ||
                                             ActivePage == NavigationPage.Diagnostic ||
                                             ActivePage == NavigationPage.AssistantTest;
    public bool IsHistoriqueActive => ActivePage == NavigationPage.Historique;
    public bool IsBaseConnaissancesActive => ActivePage == NavigationPage.BaseConnaissances;

    public string HeureActuelle
    {
        get => _heureActuelle;
        private set => SetProperty(ref _heureActuelle, value);
    }

    public string DateActuelle
    {
        get => _dateActuelle;
        private set => SetProperty(ref _dateActuelle, value);
    }

    public ICommand NaviguerAccueilCommand { get; }
    public ICommand NaviguerNouveauDiagnosticCommand { get; }
    public ICommand NaviguerReprendreCommand { get; }
    public ICommand NaviguerHistoriqueCommand { get; }
    public ICommand NaviguerBaseConnaissancesCommand { get; }

    public MainViewModel(NavigationService navigationService, IServiceProvider serviceProvider)
    {
        _navigationService = navigationService;
        _serviceProvider = serviceProvider;

        NaviguerAccueilCommand = new RelayCommand(() => _navigationService.NaviguerVers(NavigationPage.Accueil));
        NaviguerNouveauDiagnosticCommand = new RelayCommand(() => _navigationService.NaviguerVers(NavigationPage.IdentificationConsole));
        NaviguerReprendreCommand = new RelayCommand(() => _navigationService.NaviguerVers(NavigationPage.Historique));
        NaviguerHistoriqueCommand = new RelayCommand(() => _navigationService.NaviguerVers(NavigationPage.Historique));
        NaviguerBaseConnaissancesCommand = new RelayCommand(() => _navigationService.NaviguerVers(NavigationPage.BaseConnaissances));

        MettreAJourHorloge();
        _timerHorloge = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(5)
        };
        _timerHorloge.Tick += (s, e) => MettreAJourHorloge();
        _timerHorloge.Start();

        _navigationService.PageChangee += OnPageChangee;

        // Afficher l'accueil par défaut
        NaviguerVersPage(NavigationPage.Accueil, null);
    }

    private void MettreAJourHorloge()
    {
        HeureActuelle = DateTime.Now.ToString("HH:mm");
        DateActuelle = DateTime.Now.ToString("dddd d MMMM", CultureInfo.GetCultureInfo("fr-FR"));
    }

    private void OnPageChangee(NavigationPage page, object? parameter)
    {
        NaviguerVersPage(page, parameter);
    }

    public void NaviguerVersPage(NavigationPage page, object? parameter)
    {
        ActivePage = page;

        switch (page)
        {
            case NavigationPage.Accueil:
                var vmAccueil = _serviceProvider.GetRequiredService<AccueilViewModel>();
                vmAccueil.ChargerStatistiques();
                CurrentView = new AccueilView { DataContext = vmAccueil };
                break;

            case NavigationPage.IdentificationConsole:
                var vmId = _serviceProvider.GetRequiredService<IdentificationConsoleViewModel>();
                CurrentView = new IdentificationConsoleView { DataContext = vmId };
                break;

            case NavigationPage.SelectionSymptomes:
                var vmSymptomes = _serviceProvider.GetRequiredService<SelectionSymptomesViewModel>();
                vmSymptomes.Initialiser(parameter);
                CurrentView = new SelectionSymptomesView { DataContext = vmSymptomes };
                break;

            case NavigationPage.Diagnostic:
                var vmDiag = _serviceProvider.GetRequiredService<DiagnosticViewModel>();
                vmDiag.Initialiser(parameter);
                CurrentView = new DiagnosticView { DataContext = vmDiag };
                break;

            case NavigationPage.AssistantTest:
                var vmTest = _serviceProvider.GetRequiredService<AssistantTestViewModel>();
                vmTest.Initialiser(parameter);
                CurrentView = new AssistantTestView { DataContext = vmTest };
                break;

            case NavigationPage.Rapport:
                var vmRapport = _serviceProvider.GetRequiredService<RapportViewModel>();
                vmRapport.Initialiser(parameter);
                CurrentView = new RapportView { DataContext = vmRapport };
                break;

            case NavigationPage.Historique:
                var vmHist = _serviceProvider.GetRequiredService<HistoriqueViewModel>();
                vmHist.ChargerHistorique();
                CurrentView = new HistoriqueView { DataContext = vmHist };
                break;

            case NavigationPage.BaseConnaissances:
                var vmBase = _serviceProvider.GetRequiredService<BaseConnaissancesViewModel>();
                vmBase.ChargerDonnees();
                if (parameter is int tabIndex)
                {
                    vmBase.SelectedTabIndex = tabIndex;
                }
                CurrentView = new BaseConnaissancesView { DataContext = vmBase };
                break;
        }
    }
}
