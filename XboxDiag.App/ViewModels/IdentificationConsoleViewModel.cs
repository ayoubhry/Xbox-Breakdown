using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using XboxDiag.App.Models;
using XboxDiag.App.Services;
using XboxDiag.Core.Data;
using XboxDiag.Core.Models;

namespace XboxDiag.App.ViewModels;

/// <summary>
/// ViewModel pour l'étape d'identification matérielle de la console à diagnostiquer.
/// </summary>
public class IdentificationConsoleViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly DiagDbContext _dbContext;

    private ModeleConsole? _selectedModele;
    private string? _numeroSerie;
    private string? _etatGeneral;
    private string? _reparationsPrecedentes;

    public ObservableCollection<ModeleConsole> Modeles { get; } = new();

    public ModeleConsole? SelectedModele
    {
        get => _selectedModele;
        set
        {
            if (SetProperty(ref _selectedModele, value))
            {
                (SuivantCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string? NumeroSerie
    {
        get => _numeroSerie;
        set => SetProperty(ref _numeroSerie, value);
    }

    public string? EtatGeneral
    {
        get => _etatGeneral;
        set => SetProperty(ref _etatGeneral, value);
    }

    public string? ReparationsPrecedentes
    {
        get => _reparationsPrecedentes;
        set => SetProperty(ref _reparationsPrecedentes, value);
    }

    public ICommand SuivantCommand { get; }
    public ICommand RetourCommand { get; }

    public IdentificationConsoleViewModel(NavigationService navigationService, DiagDbContext dbContext)
    {
        _navigationService = navigationService;
        _dbContext = dbContext;

        SuivantCommand = new RelayCommand(Suivant, CanSuivant);
        RetourCommand = new RelayCommand(Retour);

        ChargerModeles();
    }

    private void ChargerModeles()
    {
        Modeles.Clear();
        var modelesDb = _dbContext.Modeles
            .Where(m => m.Actif)
            .OrderBy(m => m.Famille)
            .ToList();

        foreach (var m in modelesDb)
        {
            Modeles.Add(m);
        }

        SelectedModele = Modeles.FirstOrDefault();
    }

    private bool CanSuivant()
    {
        return SelectedModele != null;
    }

    private void Suivant()
    {
        if (SelectedModele == null)
            return;

        var param = new IdentificationConsoleParam
        {
            ModeleConsoleId = SelectedModele.Id,
            NomModele = SelectedModele.Nom,
            NumeroSerie = NumeroSerie?.Trim(),
            EtatGeneral = EtatGeneral?.Trim(),
            ReparationsPrecedentes = ReparationsPrecedentes?.Trim()
        };

        _navigationService.NaviguerVers(NavigationPage.SelectionSymptomes, param);
    }

    private void Retour()
    {
        _navigationService.NaviguerVers(NavigationPage.Accueil);
    }
}
