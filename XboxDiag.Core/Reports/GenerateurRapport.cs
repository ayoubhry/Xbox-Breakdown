using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using XboxDiag.Core.Models;

namespace XboxDiag.Core.Reports;

/// <summary>
/// Générateur de rapports techniques professionnels au format PDF pour les sessions de diagnostic XboxDiag.
/// </summary>
public class GenerateurRapport
{
    private static readonly Color CouleurXboxPrimaire = Color.FromHex("#107C10");
    private static readonly Color CouleurXboxFonce = Color.FromHex("#0E5A0E");
    private static readonly Color CouleurGrisFond = Color.FromHex("#F4F6F7");
    private static readonly Color CouleurBordure = Color.FromHex("#E2E8F0");

    static GenerateurRapport()
    {
        // Activation de la licence communautaire QuestPDF (Open Source / PME)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    /// <summary>
    /// Génère un document PDF complet résumant le diagnostic, les tests, les hypothèses et les recommandations.
    /// </summary>
    /// <param name="session">Entité SessionDiagnostic chargée avec toutes ses navigations.</param>
    /// <returns>Le tableau d'octets représentant le document PDF.</returns>
    public byte[] GenererRapport(SessionDiagnostic session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken3).FontFamily(Fonts.Arial));

                page.Header().Element(c => ComposerEntete(c, session));
                page.Content().Element(c => ComposerCorps(c, session));
                page.Footer().Element(c => ComposerPiedDePage(c, session));
            });
        });

        return document.GeneratePdf();
    }

    private void ComposerEntete(IContainer container, SessionDiagnostic session)
    {
        container.Column(col =>
        {
            col.Item().Row(row =>
            {
                row.RelativeItem().Column(titleCol =>
                {
                    titleCol.Item().Text("Rapport de diagnostic Xbox")
                        .FontSize(20)
                        .ExtraBold()
                        .FontColor(CouleurXboxPrimaire);

                    titleCol.Item().Text("Xbox Breakdown - Système Expert de Dépannage Matériel")
                        .FontSize(10)
                        .Italic()
                        .FontColor(Colors.Grey.Medium);
                });

                row.ConstantItem(180).Column(infoCol =>
                {
                    infoCol.Item().AlignRight().Text($"Session N° : #{session.Id:D6}")
                        .FontSize(11)
                        .SemiBold();

                    infoCol.Item().AlignRight().Text($"Date : {session.DateCreation:dd/MM/yyyy HH:mm}")
                        .FontSize(9)
                        .FontColor(Colors.Grey.Darken1);

                    infoCol.Item().AlignRight().Text($"Statut : {session.Statut}")
                        .FontSize(9)
                        .Bold()
                        .FontColor(session.Statut == StatutSession.Termine ? CouleurXboxPrimaire : Colors.Orange.Darken2);
                });
            });

            col.Item().PaddingTop(10).LineHorizontal(1.5f).LineColor(CouleurXboxPrimaire);
        });
    }

    private void ComposerCorps(IContainer container, SessionDiagnostic session)
    {
        container.PaddingVertical(10).Column(col =>
        {
            // 1. Informations Console
            col.Item().Element(c => ComposerSectionConsole(c, session));

            // 2. Symptômes initiaux
            col.Item().PaddingTop(12).Element(c => ComposerSectionSymptomes(c, session));

            // 3. Synthèse de la panne principale (Mise en avant)
            col.Item().PaddingTop(12).Element(c => ComposerSectionCausePrincipale(c, session));

            // 4. Tests diagnostiques exécutés
            col.Item().PaddingTop(12).Element(c => ComposerSectionTests(c, session));

            // 5. Tableau d'évaluation des hypothèses
            col.Item().PaddingTop(12).Element(c => ComposerSectionHypotheses(c, session));

            // 6. Dossier de réparation / Clôture si existant
            if (session.DossierReparation != null)
            {
                col.Item().PaddingTop(12).Element(c => ComposerSectionDossier(c, session.DossierReparation));
            }
        });
    }

    private void ComposerSectionConsole(IContainer container, SessionDiagnostic session)
    {
        container.Background(CouleurGrisFond).Padding(10).Column(col =>
        {
            col.Item().Text("1. INFORMATIONS CONSOLE")
                .FontSize(11)
                .Bold()
                .FontColor(CouleurXboxFonce);

            col.Item().PaddingTop(5).Row(row =>
            {
                row.RelativeItem().Text(t =>
                {
                    t.Span("Modèle : ").SemiBold();
                    t.Span(session.ModeleConsole?.Nom ?? "Inconnu");
                });

                row.RelativeItem().Text(t =>
                {
                    t.Span("Réf. Matérielle : ").SemiBold();
                    t.Span(session.ModeleConsole?.ReferenceMaterielle ?? "N/A");
                });

                row.RelativeItem().Text(t =>
                {
                    t.Span("N° Série : ").SemiBold();
                    t.Span(session.NumeroSerie ?? "Non renseigné");
                });
            });

            if (!string.IsNullOrWhiteSpace(session.EtatGeneral))
            {
                col.Item().PaddingTop(4).Text(t =>
                {
                    t.Span("État général : ").SemiBold();
                    t.Span(session.EtatGeneral);
                });
            }
        });
    }

    private void ComposerSectionSymptomes(IContainer container, SessionDiagnostic session)
    {
        container.Column(col =>
        {
            col.Item().Text("2. SYMPTÔMES SIGNALÉS")
                .FontSize(11)
                .Bold()
                .FontColor(CouleurXboxFonce);

            if (session.SessionSymptomes == null || session.SessionSymptomes.Count == 0)
            {
                col.Item().PaddingTop(4).Text("Aucun symptôme enregistré.").Italic();
                return;
            }

            col.Item().PaddingTop(4).Column(list =>
            {
                foreach (var ss in session.SessionSymptomes)
                {
                    list.Item().PaddingVertical(2).Row(r =>
                    {
                        r.ConstantItem(15).Text("•").Bold().FontColor(CouleurXboxPrimaire);
                        r.RelativeItem().Text(t =>
                        {
                            t.Span($"[{ss.Symptome.Categorie}] ").SemiBold().FontColor(Colors.Grey.Darken2);
                            t.Span(ss.Symptome.Description);
                        });
                    });
                }
            });
        });
    }

    private void ComposerSectionCausePrincipale(IContainer container, SessionDiagnostic session)
    {
        var causePrincipale = session.Hypotheses
            .Where(h => h.Statut != StatutHypothese.EcarteeParTest)
            .OrderByDescending(h => h.Statut == StatutHypothese.ConfirmeeParVerification ? 1000 : h.Confiance)
            .FirstOrDefault();

        container.Border(1.5f).BorderColor(CouleurXboxPrimaire).Background("#F0FDF4").Padding(10).Column(col =>
        {
            col.Item().Row(r =>
            {
                r.RelativeItem().Text("3. CAUSE LA PLUS PROBABLE DÉTECTÉE")
                    .FontSize(11)
                    .ExtraBold()
                    .FontColor(CouleurXboxFonce);

                if (causePrincipale != null)
                {
                    r.ConstantItem(120).AlignRight().Text($"Confiance : {causePrincipale.Confiance}%")
                        .FontSize(11)
                        .ExtraBold()
                        .FontColor(CouleurXboxPrimaire);
                }
            });

            if (causePrincipale == null)
            {
                col.Item().PaddingTop(5).Text("Aucune panne identifiée ou toutes les pistes ont été écartées.").Italic();
                return;
            }

            col.Item().PaddingTop(4).Text(causePrincipale.Panne?.Nom ?? "Panne indéterminée")
                .FontSize(13)
                .Bold()
                .FontColor(Colors.Black);

            col.Item().PaddingTop(2).Text(causePrincipale.Panne?.Description ?? string.Empty)
                .FontSize(9.5f)
                .FontColor(Colors.Grey.Darken3);

            // Recommandations selon la panne
            col.Item().PaddingTop(6).Text(t =>
            {
                t.Span("Statut du diagnostic : ").SemiBold();
                t.Span($"{causePrincipale.Statut}").Bold().FontColor(CouleurXboxPrimaire);
            });

            col.Item().PaddingTop(4).Text("Recommandations techniques :")
                .FontSize(9.5f)
                .SemiBold()
                .Underline();

            col.Item().PaddingTop(2).Text(ObtenirRecommandations(causePrincipale.Panne?.Nom))
                .FontSize(9)
                .Italic();
        });
    }

    private void ComposerSectionTests(IContainer container, SessionDiagnostic session)
    {
        container.Column(col =>
        {
            col.Item().Text("4. TESTS DE DIAGNOSTIC RÉALISÉS")
                .FontSize(11)
                .Bold()
                .FontColor(CouleurXboxFonce);

            if (session.ResultatsTests == null || session.ResultatsTests.Count == 0)
            {
                col.Item().PaddingTop(4).Text("Aucun test n'a été exécuté pour le moment.").Italic();
                return;
            }

            col.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3); // Nom test
                    columns.RelativeColumn(1.3f); // Résultat
                    columns.RelativeColumn(1.2f); // Horodatage
                    columns.RelativeColumn(2.5f); // Notes
                });

                table.Header(header =>
                {
                    header.Cell().Background(CouleurGrisFond).Padding(4).Text("Test").Bold().FontSize(9);
                    header.Cell().Background(CouleurGrisFond).Padding(4).Text("Résultat").Bold().FontSize(9);
                    header.Cell().Background(CouleurGrisFond).Padding(4).Text("Date").Bold().FontSize(9);
                    header.Cell().Background(CouleurGrisFond).Padding(4).Text("Notes du technicien").Bold().FontSize(9);
                });

                foreach (var res in session.ResultatsTests.OrderBy(r => r.Horodatage))
                {
                    var couleurResultat = res.Resultat switch
                    {
                        ResultatTest.Conforme => Colors.Green.Darken2,
                        ResultatTest.NonConforme => Colors.Red.Darken2,
                        ResultatTest.Incertain => Colors.Orange.Darken2,
                        _ => Colors.Grey.Darken2
                    };

                    table.Cell().BorderBottom(1).BorderColor(CouleurBordure).Padding(4).Text(res.TestDiagnostic?.Nom ?? $"Test #{res.TestDiagnosticId}").FontSize(8.5f);
                    table.Cell().BorderBottom(1).BorderColor(CouleurBordure).Padding(4).Text(res.Resultat.ToString()).Bold().FontColor(couleurResultat).FontSize(8.5f);
                    table.Cell().BorderBottom(1).BorderColor(CouleurBordure).Padding(4).Text(res.Horodatage.ToString("dd/MM HH:mm")).FontSize(8.5f);
                    table.Cell().BorderBottom(1).BorderColor(CouleurBordure).Padding(4).Text(res.NotesTechnicien ?? "-").FontSize(8.5f);
                }
            });
        });
    }

    private void ComposerSectionHypotheses(IContainer container, SessionDiagnostic session)
    {
        container.Column(col =>
        {
            col.Item().Text("5. SYNTHÈSE DES HYPOTHÈSES ÉVALUÉES")
                .FontSize(11)
                .Bold()
                .FontColor(CouleurXboxFonce);

            if (session.Hypotheses == null || session.Hypotheses.Count == 0)
            {
                col.Item().PaddingTop(4).Text("Aucune hypothèse calculée.").Italic();
                return;
            }

            col.Item().PaddingTop(5).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(2.5f); // Panne
                    columns.RelativeColumn(1.5f); // Statut
                    columns.RelativeColumn(1.0f); // Confiance
                    columns.RelativeColumn(3.0f); // Déductions
                });

                table.Header(header =>
                {
                    header.Cell().Background(CouleurGrisFond).Padding(4).Text("Panne envisagée").Bold().FontSize(9);
                    header.Cell().Background(CouleurGrisFond).Padding(4).Text("Statut").Bold().FontSize(9);
                    header.Cell().Background(CouleurGrisFond).Padding(4).Text("Confiance").Bold().FontSize(9);
                    header.Cell().Background(CouleurGrisFond).Padding(4).Text("Dernières déductions").Bold().FontSize(9);
                });

                foreach (var hyp in session.Hypotheses.OrderByDescending(h => h.Confiance))
                {
                    var couleurStatut = hyp.Statut switch
                    {
                        StatutHypothese.ConfirmeeParVerification => Colors.Green.Darken2,
                        StatutHypothese.RenforceeParTests => CouleurXboxPrimaire,
                        StatutHypothese.EcarteeParTest => Colors.Grey.Medium,
                        StatutHypothese.PeuProbable => Colors.Grey.Darken1,
                        _ => Colors.Blue.Darken2
                    };

                    string premiereLigneJustif = hyp.Justification
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                        .LastOrDefault() ?? "-";

                    table.Cell().BorderBottom(1).BorderColor(CouleurBordure).Padding(4).Text(hyp.Panne?.Nom ?? $"Panne #{hyp.PanneId}").FontSize(8.5f).Bold();
                    table.Cell().BorderBottom(1).BorderColor(CouleurBordure).Padding(4).Text(hyp.Statut.ToString()).FontSize(8.5f).Bold().FontColor(couleurStatut);
                    table.Cell().BorderBottom(1).BorderColor(CouleurBordure).Padding(4).Text($"{hyp.Confiance}%").FontSize(8.5f).Bold();
                    table.Cell().BorderBottom(1).BorderColor(CouleurBordure).Padding(4).Text(premiereLigneJustif).FontSize(8f);
                }
            });
        });
    }

    private void ComposerSectionDossier(IContainer container, DossierReparation dossier)
    {
        container.Background(CouleurGrisFond).Padding(10).Column(col =>
        {
            col.Item().Text("6. DOSSIER DE RÉPARATION ET CLÔTURE")
                .FontSize(11)
                .Bold()
                .FontColor(CouleurXboxFonce);

            col.Item().PaddingTop(4).Row(r =>
            {
                r.RelativeItem().Text(t =>
                {
                    t.Span("Réf. Dossier : ").SemiBold();
                    t.Span(dossier.ReferenceDossier).Bold();
                });

                r.RelativeItem().Text(t =>
                {
                    t.Span("Date Clôture : ").SemiBold();
                    t.Span(dossier.DateDiagnostic.ToString("dd/MM/yyyy HH:mm"));
                });

                r.RelativeItem().Text(t =>
                {
                    t.Span("Panne réparée : ").SemiBold();
                    t.Span(dossier.PanneIdentifiee?.Nom ?? "N/A");
                });
            });

            if (!string.IsNullOrWhiteSpace(dossier.ComposantsRepares))
            {
                col.Item().PaddingTop(3).Text(t =>
                {
                    t.Span("Composants remplacés / réparés : ").SemiBold();
                    t.Span(dossier.ComposantsRepares);
                });
            }

            if (!string.IsNullOrWhiteSpace(dossier.ResultatApresReparation))
            {
                col.Item().PaddingTop(3).Text(t =>
                {
                    t.Span("Bilan après intervention : ").SemiBold();
                    t.Span(dossier.ResultatApresReparation);
                });
            }

            if (!string.IsNullOrWhiteSpace(dossier.NotesTechnicien))
            {
                col.Item().PaddingTop(3).Text(t =>
                {
                    t.Span("Remarques finales technicien : ").SemiBold();
                    t.Span(dossier.NotesTechnicien);
                });
            }
        });
    }

    private void ComposerPiedDePage(IContainer container, SessionDiagnostic session)
    {
        container.Column(col =>
        {
            col.Item().LineHorizontal(1).LineColor(CouleurBordure);
            col.Item().PaddingTop(6).Row(row =>
            {
                row.RelativeItem().Text("Xbox Breakdown v1.0 - Système Expert de Dépannage Informatique et Électronique")
                    .FontSize(8)
                    .FontColor(Colors.Grey.Medium);

                row.RelativeItem().AlignRight().Text(text =>
                {
                    text.DefaultTextStyle(x => x.FontSize(8).FontColor(Colors.Grey.Medium));
                    text.Span("Page ");
                    text.CurrentPageNumber();
                    text.Span(" / ");
                    text.TotalPages();
                });
            });
        });
    }

    private string ObtenirRecommandations(string? nomPanne)
    {
        return nomPanne switch
        {
            "Alimentation défaillante" =>
                "Remplacement du bloc d'alimentation externe ou révision du module interne 12V (contrôle des condensateurs de filtrage primaire et secondaire). Vérifier l'absence de court-circuit sur la prise d'entrée.",

            "Câble ou diffuseur HDMI défaillant" =>
                "Remplacement du câble HDMI par un modèle certifié Premium High Speed (18 Gbps / 48 Gbps). Vérifier le port HDMI du téléviseur et désactiver l'EDID forcé.",

            "Port HDMI endommagé" =>
                "Dépose à l'air chaud (environ 380°C) du connecteur HDMI femelle endommagé. Nettoyage des pastilles à la tresse à dessouder et ressoudage d'un connecteur neuf OEM avec inspection au microscope.",

            "Circuit HDMI défectueux (Retimer / Filtres ESD)" =>
                "Remplacement de la puce Retimer (Texas Instruments TDP158 ou SN75DP159) par soudure CMS. Remplacement des 4 selfs de filtrage HDMI en cas de coupure de continuité.",

            "Lecteur optique défaillant" =>
                "Nettoyage de la lentille optique à l'alcool isopropylique. En cas d'échec persistant, remplacement du chariot laser (KEM-490AAA / DG-6M5S). Attention : conserver impérativement la carte électronique PCB fille originale sous peine d'incompatibilité avec la carte mère !",

            "Disque dur défaillant ou corrompu" =>
                "Remplacement du disque dur mécanique par un SSD SATA 2.5 (gain significatif de fluidité). Reformatage et réinstallation du système d'exploitation Xbox via une clé USB préparée avec la mise à jour système hors ligne (OSU1).",

            "Surchauffe / Ventilateur défaillant" =>
                "Dépoussiérage intégral de la turbine et du radiateur à la soufflette. Nettoyage méticuleux des résidus de pâte thermique ancienne sur le die de l'APU à l'alcool isopropylique et application d'une pâte thermique haute performance (ex. Arctic MX-4 ou Noctua NT-H1).",

            "Carte mère défaillante (Court-circuit / VRM / APU)" =>
                "Localisation précise du court-circuit à la caméra thermique sous injection de tension basse (1V / 2A). Remplacement des MOSFETs DrMOS percés ou condensateurs CMS de découplage en court-circuit.",

            _ => "Poursuivre les investigations par injection de tension contrôlée et contrôle des signaux d'horloge au multimètre et oscilloscope."
        };
    }
}
