using Microsoft.EntityFrameworkCore;
using XboxDiag.Core.Data;
using XboxDiag.Core.Models;

namespace XboxDiag.Core.Engine;

/// <summary>
/// Implémentation complète et déterministe du moteur de diagnostic expert XboxDiag.
/// </summary>
public class DiagnosticEngine : IDiagnosticEngine
{
    private readonly DiagDbContext _context;

    /// <summary>
    /// Initialise le moteur avec un contexte de base de données spécifique.
    /// </summary>
    /// <param name="context">Contexte Entity Framework DiagDbContext.</param>
    public DiagnosticEngine(DiagDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Initialise le moteur avec une nouvelle instance par défaut de DiagDbContext.
    /// </summary>
    public DiagnosticEngine() : this(new DiagDbContext())
    {
    }

    /// <inheritdoc />
    public SessionDiagnostic DemarrerSession(int modeleConsoleId, List<int> symptomeIds)
    {
        var modele = _context.Modeles.Find(modeleConsoleId)
            ?? throw new ArgumentException($"Le modèle de console ID {modeleConsoleId} n'existe pas.", nameof(modeleConsoleId));

        var session = new SessionDiagnostic
        {
            ModeleConsoleId = modeleConsoleId,
            DateCreation = DateTime.UtcNow,
            Statut = StatutSession.EnCours,
            Notes = $"Session démarrée le {DateTime.Now:dd/MM/yyyy HH:mm} pour {modele.Nom}."
        };

        _context.Sessions.Add(session);
        _context.SaveChanges();

        // Enregistrement des symptômes observés
        var distinctSymptomeIds = symptomeIds.Distinct().ToList();
        foreach (var sid in distinctSymptomeIds)
        {
            _context.SessionSymptomes.Add(new SessionSymptome
            {
                SessionDiagnosticId = session.Id,
                SymptomeId = sid
            });
        }
        _context.SaveChanges();

        // Génération des hypothèses initiales déduites des symptômes
        InitialiserHypotheses(session.Id, distinctSymptomeIds);

        return ReprendreSession(session.Id)!;
    }

    /// <inheritdoc />
    public List<SessionHypothese> ObtenirHypotheses(int sessionId)
    {
        return _context.Hypotheses
            .Include(h => h.Panne)
            .Where(h => h.SessionDiagnosticId == sessionId)
            .OrderByDescending(h => h.Confiance)
            .ThenBy(h => h.Panne.Nom)
            .ToList();
    }

    /// <inheritdoc />
    public TestDiagnostic? ObtenirProchainTest(int sessionId)
    {
        var session = _context.Sessions
            .Include(s => s.ResultatsTests)
            .FirstOrDefault(s => s.Id == sessionId)
            ?? throw new ArgumentException($"Session ID {sessionId} introuvable.", nameof(sessionId));

        var testsEffectuesIds = session.ResultatsTests
            .Select(r => r.TestDiagnosticId)
            .ToHashSet();

        // Hypothèses non éliminées encore en jeu
        var hypothesesActives = _context.Hypotheses
            .Where(h => h.SessionDiagnosticId == sessionId && h.Statut != StatutHypothese.EcarteeParTest)
            .ToList();

        if (hypothesesActives.Count == 0)
        {
            return null;
        }

        var pannesActivesIds = hypothesesActives.Select(h => h.PanneId).ToHashSet();

        // Récupérer tous les tests actifs disponibles pour ce modèle non encore réalisés
        var testsDisponibles = _context.Tests
            .Include(t => t.Regles)
            .Where(t => t.Actif &&
                        (t.ModeleConsoleId == null || t.ModeleConsoleId == session.ModeleConsoleId) &&
                        !testsEffectuesIds.Contains(t.Id))
            .ToList();

        if (testsDisponibles.Count == 0)
        {
            return null;
        }

        // On classe les tests par :
        // 1. Nombre d'hypothèses actives ciblées par ses règles (pouvoir discriminant maximal)
        // 2. Niveau de difficulté croissant (Débutant en premier, puis Intermédiaire, puis Expert)
        // 3. Ordre d'identifiant stable
        var testChoisi = testsDisponibles
            .Select(t => new
            {
                Test = t,
                NbHypothesesImpactees = t.Regles.Count(r => pannesActivesIds.Contains(r.PanneId))
            })
            .Where(x => x.NbHypothesesImpactees > 0)
            .OrderByDescending(x => x.NbHypothesesImpactees)
            .ThenBy(x => (int)x.Test.Difficulte)
            .ThenBy(x => x.Test.Id)
            .Select(x => x.Test)
            .FirstOrDefault();

        // Si aucun test restant n'a de règle ciblant directement les pannes actives,
        // on propose le test disponible le plus simple
        return testChoisi ?? testsDisponibles
            .OrderBy(t => (int)t.Difficulte)
            .ThenBy(t => t.Id)
            .FirstOrDefault();
    }

    /// <inheritdoc />
    public void EnregistrerResultatTest(int sessionId, int testId, ResultatTest resultat, string? notes)
    {
        var session = _context.Sessions.Find(sessionId)
            ?? throw new ArgumentException($"Session ID {sessionId} introuvable.", nameof(sessionId));

        var test = _context.Tests.Find(testId)
            ?? throw new ArgumentException($"Test ID {testId} introuvable.", nameof(testId));

        var resultatTest = new SessionResultatTest
        {
            SessionDiagnosticId = sessionId,
            TestDiagnosticId = testId,
            Resultat = resultat,
            Horodatage = DateTime.UtcNow,
            NotesTechnicien = notes
        };

        _context.ResultatsTests.Add(resultatTest);
        _context.SaveChanges();

        // Mise à jour immédiate des déductions
        MettreAJourHypotheses(sessionId);
    }

    /// <inheritdoc />
    public void MettreAJourHypotheses(int sessionId)
    {
        var session = _context.Sessions
            .Include(s => s.SessionSymptomes)
            .Include(s => s.ResultatsTests)
                .ThenInclude(r => r.TestDiagnostic)
            .Include(s => s.Hypotheses)
                .ThenInclude(h => h.Panne)
            .FirstOrDefault(s => s.Id == sessionId)
            ?? throw new ArgumentException($"Session ID {sessionId} introuvable.", nameof(sessionId));

        var symptomeIds = session.SessionSymptomes.Select(ss => ss.SymptomeId).ToList();

        // Récupération de tous les liens PanneSymptome pour ces symptômes
        var panneSymptomes = _context.PanneSymptomes
            .Where(ps => symptomeIds.Contains(ps.SymptomeId))
            .ToList();

        // Récupération de toutes les règles associées aux tests exécutés
        var testIdsExecutes = session.ResultatsTests.Select(r => r.TestDiagnosticId).ToList();
        var reglesApplicables = _context.Regles
            .Include(r => r.Test)
            .Where(r => testIdsExecutes.Contains(r.TestDiagnosticId))
            .ToList();

        foreach (var hypothese in session.Hypotheses)
        {
            // 1. Calcul de la confiance de base à partir des symptômes
            var matchingPS = panneSymptomes.Where(ps => ps.PanneId == hypothese.PanneId).ToList();
            int scoreSymptomes = matchingPS.Sum(ps => ps.Poids);
            int confianceBase = Math.Clamp(scoreSymptomes * 5, 20, 75);

            int confianceCourante = confianceBase;
            bool estEliminee = false;
            bool estConfirmee = false;
            bool estRenforcee = false;

            var journalExplications = new List<string>
            {
                $"Confiance initiale basée sur {matchingPS.Count} symptôme(s) concordant(s) (score : {scoreSymptomes}, confiance base : {confianceBase}%)."
            };

            // 2. Application séquentielle des résultats de tests
            foreach (var res in session.ResultatsTests.OrderBy(r => r.Horodatage))
            {
                var reglesPourCeTest = reglesApplicables
                    .Where(r => r.TestDiagnosticId == res.TestDiagnosticId &&
                                r.PanneId == hypothese.PanneId &&
                                r.ResultatAttendu == res.Resultat)
                    .ToList();

                foreach (var regle in reglesPourCeTest)
                {
                    confianceCourante += regle.DeltaConfiance;

                    string detailRegle = $"[Test: {res.TestDiagnostic.Nom} | Résultat: {res.Resultat}] -> Règle {regle.Effet} ({regle.DeltaConfiance:+0;-0;0}%)";
                    if (!string.IsNullOrWhiteSpace(regle.Commentaire))
                    {
                        detailRegle += $" : {regle.Commentaire}";
                    }
                    journalExplications.Add(detailRegle);

                    switch (regle.Effet)
                    {
                        case EffetRegle.Eliminer:
                            estEliminee = true;
                            confianceCourante = 0;
                            break;

                        case EffetRegle.Confirmer:
                            estConfirmee = true;
                            confianceCourante = Math.Max(confianceCourante, 95);
                            break;

                        case EffetRegle.Renforcer:
                            estRenforcee = true;
                            break;

                        case EffetRegle.Affaiblir:
                            break;
                    }
                }
            }

            // Normalisation de la confiance
            confianceCourante = Math.Clamp(confianceCourante, 0, 100);
            hypothese.Confiance = confianceCourante;

            // 3. Attribution du statut conformément aux spécifications
            if (estEliminee || confianceCourante == 0)
            {
                hypothese.Statut = StatutHypothese.EcarteeParTest;
                hypothese.Confiance = 0;
            }
            else if (estConfirmee)
            {
                hypothese.Statut = StatutHypothese.ConfirmeeParVerification;
            }
            else if (confianceCourante < 20)
            {
                hypothese.Statut = StatutHypothese.PeuProbable;
            }
            else if (confianceCourante > 80 && estRenforcee)
            {
                hypothese.Statut = StatutHypothese.RenforceeParTests;
            }
            else
            {
                hypothese.Statut = StatutHypothese.CompatibleSymptomes;
            }

            hypothese.Justification = string.Join("\n• ", journalExplications);
        }

        _context.SaveChanges();
    }

    /// <inheritdoc />
    public void AjouterSymptome(int sessionId, int symptomeId)
    {
        var existe = _context.SessionSymptomes
            .Any(ss => ss.SessionDiagnosticId == sessionId && ss.SymptomeId == symptomeId);

        if (!existe)
        {
            _context.SessionSymptomes.Add(new SessionSymptome
            {
                SessionDiagnosticId = sessionId,
                SymptomeId = symptomeId
            });
            _context.SaveChanges();

            // Vérifier si de nouvelles pannes liées doivent être ajoutées comme hypothèses
            var session = _context.Sessions
                .Include(s => s.Hypotheses)
                .First(s => s.Id == sessionId);

            var pannesExistantesIds = session.Hypotheses.Select(h => h.PanneId).ToHashSet();

            var nouvellesPannesLiees = _context.PanneSymptomes
                .Include(ps => ps.Panne)
                .Where(ps => ps.SymptomeId == symptomeId &&
                             !pannesExistantesIds.Contains(ps.PanneId) &&
                             (ps.Panne.ModeleConsoleId == null || ps.Panne.ModeleConsoleId == session.ModeleConsoleId))
                .Select(ps => ps.Panne)
                .Distinct()
                .ToList();

            foreach (var panne in nouvellesPannesLiees)
            {
                _context.Hypotheses.Add(new SessionHypothese
                {
                    SessionDiagnosticId = sessionId,
                    PanneId = panne.Id,
                    Statut = StatutHypothese.CompatibleSymptomes,
                    Confiance = 30,
                    Justification = "Hypothèse ajoutée suite à l'observation d'un symptôme complémentaire."
                });
            }
            _context.SaveChanges();
        }

        MettreAJourHypotheses(sessionId);
    }

    /// <inheritdoc />
    public void AnnulerDernierTest(int sessionId)
    {
        var dernierTest = _context.ResultatsTests
            .Where(r => r.SessionDiagnosticId == sessionId)
            .OrderByDescending(r => r.Horodatage)
            .ThenByDescending(r => r.Id)
            .FirstOrDefault();

        if (dernierTest != null)
        {
            _context.ResultatsTests.Remove(dernierTest);
            _context.SaveChanges();
            MettreAJourHypotheses(sessionId);
        }
    }

    /// <inheritdoc />
    public void RecommencerDiagnostic(int sessionId)
    {
        var session = _context.Sessions
            .Include(s => s.ResultatsTests)
            .Include(s => s.Hypotheses)
            .Include(s => s.SessionSymptomes)
            .FirstOrDefault(s => s.Id == sessionId)
            ?? throw new ArgumentException($"Session ID {sessionId} introuvable.", nameof(sessionId));

        _context.ResultatsTests.RemoveRange(session.ResultatsTests);
        _context.Hypotheses.RemoveRange(session.Hypotheses);
        _context.SaveChanges();

        session.Statut = StatutSession.EnCours;
        _context.SaveChanges();

        var symptomeIds = session.SessionSymptomes.Select(ss => ss.SymptomeId).ToList();
        InitialiserHypotheses(sessionId, symptomeIds);
    }

    /// <inheritdoc />
    public SessionDiagnostic? ReprendreSession(int sessionId)
    {
        return _context.Sessions
            .Include(s => s.ModeleConsole)
            .Include(s => s.SessionSymptomes)
                .ThenInclude(ss => ss.Symptome)
            .Include(s => s.ResultatsTests)
                .ThenInclude(r => r.TestDiagnostic)
            .Include(s => s.Hypotheses)
                .ThenInclude(h => h.Panne)
            .Include(s => s.DossierReparation)
                .ThenInclude(d => d!.PanneIdentifiee)
            .FirstOrDefault(s => s.Id == sessionId);
    }

    /// <inheritdoc />
    public List<SessionDiagnostic> ObtenirHistorique()
    {
        return _context.Sessions
            .Include(s => s.ModeleConsole)
            .Include(s => s.DossierReparation)
            .OrderByDescending(s => s.DateCreation)
            .ToList();
    }

    /// <inheritdoc />
    public DossierReparation CloturerDossier(int sessionId, string? composantsRepares, string? resultatReparation, string? notes)
    {
        var session = ReprendreSession(sessionId)
            ?? throw new ArgumentException($"Session ID {sessionId} introuvable.", nameof(sessionId));

        session.Statut = StatutSession.Termine;

        // Détermination de la panne principale la plus probable
        var panneLaPlusProbable = session.Hypotheses
            .Where(h => h.Statut != StatutHypothese.EcarteeParTest)
            .OrderByDescending(h => h.Statut == StatutHypothese.ConfirmeeParVerification ? 1000 : h.Confiance)
            .FirstOrDefault();

        string reference = $"REP-{DateTime.UtcNow:yyyyMMdd}-{sessionId:D4}";

        var dossier = session.DossierReparation;
        if (dossier == null)
        {
            dossier = new DossierReparation
            {
                SessionDiagnosticId = sessionId,
                ReferenceDossier = reference,
                DateDiagnostic = DateTime.UtcNow,
                PanneIdentifieeId = panneLaPlusProbable?.PanneId,
                ComposantsRepares = composantsRepares,
                ResultatApresReparation = resultatReparation,
                NotesTechnicien = notes
            };
            _context.DossiersReparation.Add(dossier);
        }
        else
        {
            dossier.DateDiagnostic = DateTime.UtcNow;
            dossier.PanneIdentifieeId = panneLaPlusProbable?.PanneId;
            dossier.ComposantsRepares = composantsRepares;
            dossier.ResultatApresReparation = resultatReparation;
            dossier.NotesTechnicien = notes;
        }

        _context.SaveChanges();
        return dossier;
    }

    /// <summary>
    /// Crée les hypothèses initiales calculées à partir des symptômes choisis.
    /// </summary>
    private void InitialiserHypotheses(int sessionId, List<int> distinctSymptomeIds)
    {
        var session = _context.Sessions.Find(sessionId)!;

        // Pannes associées aux symptômes sélectionnés
        var liens = _context.PanneSymptomes
            .Include(ps => ps.Panne)
            .Where(ps => distinctSymptomeIds.Contains(ps.SymptomeId) &&
                         (ps.Panne.ModeleConsoleId == null || ps.Panne.ModeleConsoleId == session.ModeleConsoleId))
            .ToList();

        var groupesParPanne = liens.GroupBy(l => l.PanneId);

        foreach (var groupe in groupesParPanne)
        {
            var panne = groupe.First().Panne;
            int scoreCumule = groupe.Sum(l => l.Poids);
            int confiance = Math.Clamp(scoreCumule * 5, 20, 75);

            var hypothese = new SessionHypothese
            {
                SessionDiagnosticId = sessionId,
                PanneId = panne.Id,
                Statut = StatutHypothese.CompatibleSymptomes,
                Confiance = confiance,
                Justification = $"Hypothèse identifiée d'après {groupe.Count()} symptôme(s) associé(s) (poids cumulé : {scoreCumule})."
            };

            _context.Hypotheses.Add(hypothese);
        }

        _context.SaveChanges();
    }
}
