using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using XboxDiag.App.Services;
using XboxDiag.Core.Data;
using XboxDiag.Core.Engine;
using XboxDiag.Core.Models;

namespace XboxDiag.App.ViewModels;

/// <summary>
/// Élément d'affichage pour une ligne d'historique de diagnostic dans le tableau.
/// </summary>
public class SessionHistoriqueItemViewModel
{
    public SessionDiagnostic Session { get; }
    public int Id => Session.Id;
    public DateTime DateCreation => Session.DateCreation.ToLocalTime();
    public string DateCreationTexte => DateCreation.ToString("dd/MM/yyyy HH:mm");
    public string ModeleNom => Session.ModeleConsole?.Nom ?? "Xbox";
    public string NumeroSerie => string.IsNullOrWhiteSpace(Session.NumeroSerie) ? "-" : Session.NumeroSerie;
    public StatutSession Statut => Session.Statut;
    public int NombreTests => Session.ResultatsTests?.Count ?? 0;
    public string? DossierRef => Session.DossierReparation?.ReferenceDossier;
    public string PanneTitre => Session.DossierReparation?.PanneIdentifiee?.Nom ??
                                Session.Hypotheses?.OrderByDescending(h => h.Confiance).FirstOrDefault()?.Panne?.Nom ?? "-";

    public SessionHistoriqueItemViewModel(SessionDiagnostic session)
    {
        Session = session;
    }
}

/// <summary>
/// ViewModel gérant l'historique complet des sessions de diagnostic et leur reprise.
/// </summary>
public class HistoriqueViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly IDiagnosticEngine _diagnosticEngine;
    private readonly DiagDbContext _dbContext;

    private List<SessionDiagnostic> _toutesLesSessions = new();
    private SessionHistoriqueItemViewModel? _selectedSession;
    private string _texteRecherche = string.Empty;
    private string _filtreStatut = "Tous";

    public ObservableCollection<SessionHistoriqueItemViewModel> SessionsAffichees { get; } = new();

    public SessionHistoriqueItemViewModel? SelectedSession
    {
        get => _selectedSession;
        set
        {
            if (SetProperty(ref _selectedSession, value))
            {
                (ReprendreCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (VoirRapportCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (SupprimerCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }
    }

    public string TexteRecherche
    {
        get => _texteRecherche;
        set
        {
            if (SetProperty(ref _texteRecherche, value))
            {
                AppliquerFiltres();
            }
        }
    }

    public string FiltreStatut
    {
        get => _filtreStatut;
        set
        {
            if (SetProperty(ref _filtreStatut, value))
            {
                AppliquerFiltres();
            }
        }
    }

    public ObservableCollection<string> StatutsDisponibles { get; } = new()
    {
        "Tous",
        "En cours",
        "Terminé",
        "Abandonné"
    };

    public ICommand ReprendreCommand { get; }
    public ICommand VoirRapportCommand { get; }
    public ICommand SupprimerCommand { get; }
    public ICommand NouveauDiagnosticCommand { get; }
    public ICommand RetourAccueilCommand { get; }
    public ICommand ReinitialiserFiltresCommand { get; }
    public ICommand SauvegarderBaseCommand { get; }
    public ICommand RestaurerBaseCommand { get; }
    public ICommand ExporterCsvCommand { get; }

    public HistoriqueViewModel(NavigationService navigationService, IDiagnosticEngine diagnosticEngine, DiagDbContext dbContext)
    {
        _navigationService = navigationService;
        _diagnosticEngine = diagnosticEngine;
        _dbContext = dbContext;

        ReprendreCommand = new RelayCommand(Reprendre, () => SelectedSession != null);
        VoirRapportCommand = new RelayCommand(VoirRapport, () => SelectedSession != null);
        SupprimerCommand = new RelayCommand(Supprimer, () => SelectedSession != null);
        NouveauDiagnosticCommand = new RelayCommand(NouveauDiagnostic);
        RetourAccueilCommand = new RelayCommand(RetourAccueil);
        ReinitialiserFiltresCommand = new RelayCommand(ReinitialiserFiltres);
        SauvegarderBaseCommand = new RelayCommand(SauvegarderBase);
        RestaurerBaseCommand = new RelayCommand(RestaurerBase);
        ExporterCsvCommand = new RelayCommand(ExporterCsv);
    }

    public void ChargerHistorique()
    {
        _toutesLesSessions = _dbContext.Sessions
            .Include(s => s.ModeleConsole)
            .Include(s => s.ResultatsTests)
            .Include(s => s.Hypotheses)
                .ThenInclude(h => h.Panne)
            .Include(s => s.DossierReparation)
                .ThenInclude(d => d!.PanneIdentifiee)
            .OrderByDescending(s => s.DateCreation)
            .ToList();

        AppliquerFiltres();
    }

    private void AppliquerFiltres()
    {
        SessionsAffichees.Clear();

        var query = _toutesLesSessions.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(_texteRecherche))
        {
            string search = _texteRecherche.Trim().ToLowerInvariant();
            query = query.Where(s =>
                s.Id.ToString().Contains(search) ||
                (s.NumeroSerie != null && s.NumeroSerie.ToLowerInvariant().Contains(search)) ||
                (s.ModeleConsole != null && s.ModeleConsole.Nom.ToLowerInvariant().Contains(search)) ||
                (s.DossierReparation != null && s.DossierReparation.ReferenceDossier.ToLowerInvariant().Contains(search)) ||
                (s.Notes != null && s.Notes.ToLowerInvariant().Contains(search)));
        }

        if (_filtreStatut != "Tous")
        {
            var statutRecherche = _filtreStatut switch
            {
                "En cours" => StatutSession.EnCours,
                "Terminé" => StatutSession.Termine,
                "Abandonné" => StatutSession.Abandonne,
                _ => (StatutSession?)null
            };

            if (statutRecherche.HasValue)
            {
                query = query.Where(s => s.Statut == statutRecherche.Value);
            }
        }

        foreach (var session in query)
        {
            SessionsAffichees.Add(new SessionHistoriqueItemViewModel(session));
        }

        SelectedSession = SessionsAffichees.FirstOrDefault();
    }

    private void ReinitialiserFiltres()
    {
        _texteRecherche = string.Empty;
        _filtreStatut = "Tous";
        OnPropertyChanged(nameof(TexteRecherche));
        OnPropertyChanged(nameof(FiltreStatut));
        AppliquerFiltres();
    }

    private void Reprendre()
    {
        if (SelectedSession == null)
            return;

        _navigationService.NaviguerVers(NavigationPage.Diagnostic, SelectedSession.Id);
    }

    private void VoirRapport()
    {
        if (SelectedSession == null)
            return;

        _navigationService.NaviguerVers(NavigationPage.Rapport, SelectedSession.Id);
    }

    private void Supprimer()
    {
        if (SelectedSession == null)
            return;

        var rep = MessageBox.Show(
            $"Confirmez-vous la suppression définitive de la session N° {SelectedSession.Id} ? Cette opération est irréversible.",
            "Suppression de session",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (rep == MessageBoxResult.Yes)
        {
            var sessionDb = _dbContext.Sessions.Find(SelectedSession.Id);
            if (sessionDb != null)
            {
                _dbContext.Sessions.Remove(sessionDb);
                _dbContext.SaveChanges();
            }

            ChargerHistorique();
        }
    }

    private void SauvegarderBase()
    {
        try
        {
            var dialog = new SaveFileDialog
            {
                Title = "Exporter la sauvegarde de la base de données XboxDiag",
                Filter = "Base SQLite (*.db;*.bak)|*.db;*.bak|Tous les fichiers (*.*)|*.*",
                FileName = $"xboxdiag_sauvegarde_{DateTime.Now:yyyyMMdd_HHmm}.db"
            };

            if (dialog.ShowDialog() == true)
            {
                long octets = GestionnaireSauvegarde.Sauvegarder(dialog.FileName);
                MessageBox.Show(
                    $"La base de données a été sauvegardée avec succès !\n\nDestination : {dialog.FileName}\nTaille : {octets / 1024.0:F1} Ko",
                    "Sauvegarde réussie",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors de la sauvegarde de la base de données :\n{ex.Message}",
                "Erreur de sauvegarde",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void RestaurerBase()
    {
        try
        {
            var dialog = new OpenFileDialog
            {
                Title = "Sélectionner une sauvegarde de base de données à restaurer",
                Filter = "Base SQLite (*.db;*.bak)|*.db;*.bak|Tous les fichiers (*.*)|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                var rep = MessageBox.Show(
                    $"ATTENTION : La restauration remplacera l'ensemble des diagnostics actuels par ceux de la sauvegarde sélectionnée :\n{dialog.FileName}\n\nUne copie de secours de votre base actuelle sera créée automatiquement avant écrasement.\n\nVoulez-vous continuer ?",
                    "Confirmation de restauration",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (rep == MessageBoxResult.Yes)
                {
                    GestionnaireSauvegarde.Restaurer(dialog.FileName);
                    ChargerHistorique();

                    MessageBox.Show(
                        "La base de données a été restaurée avec succès et les diagnostics ont été rechargés.",
                        "Restauration terminée",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors de la restauration de la base de données :\n{ex.Message}",
                "Erreur de restauration",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ExporterCsv()
    {
        try
        {
            if (SessionsAffichees.Count == 0)
            {
                MessageBox.Show("Aucune session à exporter.", "Exportation CSV", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new SaveFileDialog
            {
                Title = "Exporter l'historique en fichier CSV (compatible Excel)",
                Filter = "Feuille de calcul CSV (*.csv)|*.csv",
                FileName = $"Export_Diagnostics_XboxBreakdown_{DateTime.Now:yyyyMMdd_HHmm}.csv"
            };

            if (dialog.ShowDialog() == true)
            {
                var sb = new StringBuilder();
                sb.AppendLine("ID_Session;Date_Creation;Modele_Console;Numero_Serie;Statut;Nombre_Tests;Panne_Detectee;Dossier_Reparation");

                foreach (var item in SessionsAffichees)
                {
                    string date = item.DateCreationTexte;
                    string modele = (item.ModeleNom ?? "").Replace(";", ",");
                    string numSerie = (item.NumeroSerie ?? "").Replace(";", ",");
                    string statut = item.Statut.ToString();
                    string nbTests = item.NombreTests.ToString();
                    string panne = (item.PanneTitre ?? "").Replace(";", ",");
                    string dossier = (item.DossierRef ?? "").Replace(";", ",");

                    sb.AppendLine($"{item.Id};{date};{modele};{numSerie};{statut};{nbTests};{panne};{dossier}");
                }

                File.WriteAllText(dialog.FileName, sb.ToString(), new UTF8Encoding(encoderShouldEmitUTF8Identifier: true));

                var rep = MessageBox.Show(
                    $"{SessionsAffichees.Count} ligne(s) d'historique exportée(s) avec succès dans :\n{dialog.FileName}\n\nSouhaitez-vous ouvrir le fichier maintenant ?",
                    "Exportation CSV terminée",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (rep == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = dialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Erreur lors de l'exportation CSV :\n{ex.Message}",
                "Erreur d'exportation",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void NouveauDiagnostic()
    {
        _navigationService.NaviguerVers(NavigationPage.IdentificationConsole);
    }

    private void RetourAccueil()
    {
        _navigationService.NaviguerVers(NavigationPage.Accueil);
    }
}
