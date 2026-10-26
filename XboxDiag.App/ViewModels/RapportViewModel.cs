using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using XboxDiag.App.Services;
using XboxDiag.Core.Engine;
using XboxDiag.Core.Models;
using XboxDiag.Core.Reports;

namespace XboxDiag.App.ViewModels;

/// <summary>
/// ViewModel gérant la restitution du bilan diagnostic, l'exportation PDF et la clôture du dossier de réparation.
/// </summary>
public class RapportViewModel : BaseViewModel
{
    private readonly NavigationService _navigationService;
    private readonly IDiagnosticEngine _diagnosticEngine;
    private readonly GenerateurRapport _generateurRapport;

    private int _sessionId;
    private SessionDiagnostic? _session;
    private SessionHypothese? _causePrincipale;
    private string? _composantsRepares;
    private string? _resultatReparation;
    private string? _notesTechnicien;
    private bool _estCloture;
    private string? _referenceDossier;

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

    public SessionHypothese? CausePrincipale
    {
        get => _causePrincipale;
        private set
        {
            if (SetProperty(ref _causePrincipale, value))
            {
                OnPropertyChanged(nameof(NomCausePrincipale));
                OnPropertyChanged(nameof(DescriptionCausePrincipale));
                OnPropertyChanged(nameof(ConfianceCausePrincipale));
                OnPropertyChanged(nameof(HasCausePrincipale));
            }
        }
    }

    public bool HasCausePrincipale => CausePrincipale != null;
    public string NomCausePrincipale => CausePrincipale?.Panne?.Nom ?? "Panne indéterminée";
    public string DescriptionCausePrincipale => CausePrincipale?.Panne?.Description ?? "Aucune panne n'a pu être formellement discriminée.";
    public int ConfianceCausePrincipale => CausePrincipale?.Confiance ?? 0;

    public string NomModele => Session?.ModeleConsole?.Nom ?? "Xbox";
    public string NumeroSerie => string.IsNullOrWhiteSpace(Session?.NumeroSerie) ? "Non renseigné" : Session.NumeroSerie;
    public string DateCreationTexte => Session?.DateCreation.ToLocalTime().ToString("dd/MM/yyyy HH:mm") ?? "-";
    public string StatutTexte => Session?.Statut.ToString() ?? "-";

    public string? ComposantsRepares
    {
        get => _composantsRepares;
        set => SetProperty(ref _composantsRepares, value);
    }

    public string? ResultatReparation
    {
        get => _resultatReparation;
        set => SetProperty(ref _resultatReparation, value);
    }

    public string? NotesTechnicien
    {
        get => _notesTechnicien;
        set => SetProperty(ref _notesTechnicien, value);
    }

    public bool EstCloture
    {
        get => _estCloture;
        private set => SetProperty(ref _estCloture, value);
    }

    public string? ReferenceDossier
    {
        get => _referenceDossier;
        private set => SetProperty(ref _referenceDossier, value);
    }

    public ObservableCollection<Symptome> Symptomes { get; } = new();
    public ObservableCollection<SessionResultatTest> ResultatsTests { get; } = new();
    public ObservableCollection<SessionHypothese> Hypotheses { get; } = new();

    public ICommand ExporterPDFCommand { get; }
    public ICommand ApercuPDFCommand { get; }
    public ICommand CloturerDossierCommand { get; }
    public ICommand NouveauDiagnosticCommand { get; }
    public ICommand RetourDiagnosticCommand { get; }

    public RapportViewModel(NavigationService navigationService, IDiagnosticEngine diagnosticEngine, GenerateurRapport generateurRapport)
    {
        _navigationService = navigationService;
        _diagnosticEngine = diagnosticEngine;
        _generateurRapport = generateurRapport;

        ExporterPDFCommand = new RelayCommand(ExporterPDF, () => Session != null);
        ApercuPDFCommand = new RelayCommand(ApercuPDF, () => Session != null);
        CloturerDossierCommand = new RelayCommand(CloturerDossier, () => !EstCloture);
        NouveauDiagnosticCommand = new RelayCommand(NouveauDiagnostic);
        RetourDiagnosticCommand = new RelayCommand(RetourDiagnostic);
    }

    public void Initialiser(object? parameter)
    {
        if (parameter is int sid)
        {
            SessionId = sid;
            ChargerDonnees();
        }
    }

    public void ChargerDonnees()
    {
        Session = _diagnosticEngine.ReprendreSession(SessionId);
        if (Session == null)
            return;

        // Hypothèse principale
        CausePrincipale = Session.Hypotheses
            .Where(h => h.Statut != StatutHypothese.EcarteeParTest)
            .OrderByDescending(h => h.Statut == StatutHypothese.ConfirmeeParVerification ? 1000 : h.Confiance)
            .FirstOrDefault();

        // Symptômes
        Symptomes.Clear();
        foreach (var ss in Session.SessionSymptomes)
        {
            if (ss.Symptome != null)
                Symptomes.Add(ss.Symptome);
        }

        // Tests
        ResultatsTests.Clear();
        foreach (var r in Session.ResultatsTests.OrderBy(r => r.Horodatage))
        {
            ResultatsTests.Add(r);
        }

        // Hypothèses
        Hypotheses.Clear();
        foreach (var h in Session.Hypotheses.OrderByDescending(h => h.Confiance))
        {
            Hypotheses.Add(h);
        }

        // État du dossier
        if (Session.DossierReparation != null)
        {
            EstCloture = true;
            ReferenceDossier = Session.DossierReparation.ReferenceDossier;
            ComposantsRepares = Session.DossierReparation.ComposantsRepares;
            ResultatReparation = Session.DossierReparation.ResultatApresReparation;
            NotesTechnicien = Session.DossierReparation.NotesTechnicien;
        }
        else
        {
            EstCloture = false;
            ReferenceDossier = null;
        }

        (CloturerDossierCommand as RelayCommand)?.RaiseCanExecuteChanged();
    }

    private void ExporterPDF()
    {
        if (Session == null)
            return;

        try
        {
            var saveDialog = new SaveFileDialog
            {
                Title = "Enregistrer le rapport de diagnostic PDF",
                Filter = "Document PDF (*.pdf)|*.pdf",
                FileName = $"Rapport_XboxBreakdown_Session_{Session.Id:D4}_{DateTime.Now:yyyyMMdd_HHmm}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                byte[] pdfBytes = _generateurRapport.GenererRapport(Session);
                File.WriteAllBytes(saveDialog.FileName, pdfBytes);

                var rep = MessageBox.Show(
                    $"Le rapport a été généré avec succès dans :\n{saveDialog.FileName}\n\nSouhaitez-vous l'ouvrir maintenant ?",
                    "Exportation PDF réussie",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (rep == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = saveDialog.FileName,
                        UseShellExecute = true
                    });
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Une erreur est survenue lors de la génération du document PDF :\n{ex.Message}",
                "Erreur d'exportation",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void ApercuPDF()
    {
        if (Session == null)
            return;

        try
        {
            byte[] pdfBytes = _generateurRapport.GenererRapport(Session);
            string tempPath = Path.Combine(Path.GetTempPath(), $"Rapport_XboxBreakdown_Session_{Session.Id:D4}_{DateTime.Now:yyyyMMdd_HHmm}.pdf");
            File.WriteAllBytes(tempPath, pdfBytes);

            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = tempPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Impossible de prévisualiser le document PDF :\n{ex.Message}",
                "Erreur d'aperçu",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private void CloturerDossier()
    {
        var dossier = _diagnosticEngine.CloturerDossier(
            SessionId,
            ComposantsRepares?.Trim(),
            ResultatReparation?.Trim(),
            NotesTechnicien?.Trim());

        EstCloture = true;
        ReferenceDossier = dossier.ReferenceDossier;
        (CloturerDossierCommand as RelayCommand)?.RaiseCanExecuteChanged();

        MessageBox.Show(
            $"Le diagnostic est clôturé. Le dossier de réparation porte la référence :\n{dossier.ReferenceDossier}",
            "Dossier clôturé",
            MessageBoxButton.OK,
            MessageBoxImage.Information);

        ChargerDonnees();
    }

    private void NouveauDiagnostic()
    {
        _navigationService.NaviguerVers(NavigationPage.IdentificationConsole);
    }

    private void RetourDiagnostic()
    {
        _navigationService.NaviguerVers(NavigationPage.Diagnostic, SessionId);
    }
}
