using Microsoft.EntityFrameworkCore;
using XboxDiag.Core.Models;

namespace XboxDiag.Core.Data;

/// <summary>
/// Contexte Entity Framework Core pour la gestion des données de diagnostic XboxDiag.
/// </summary>
public class DiagDbContext : DbContext
{
    /// <summary>
    /// Modèles de consoles Xbox.
    /// </summary>
    public DbSet<ModeleConsole> Modeles => Set<ModeleConsole>();

    /// <summary>
    /// Catalogue des symptômes observables.
    /// </summary>
    public DbSet<Symptome> Symptomes => Set<Symptome>();

    /// <summary>
    /// Catalogue des pannes répertoriées.
    /// </summary>
    public DbSet<Panne> Pannes => Set<Panne>();

    /// <summary>
    /// Liaisons pondérées entre pannes et symptômes.
    /// </summary>
    public DbSet<PanneSymptome> PanneSymptomes => Set<PanneSymptome>();

    /// <summary>
    /// Catalogue des tests techniques de diagnostic.
    /// </summary>
    public DbSet<TestDiagnostic> Tests => Set<TestDiagnostic>();

    /// <summary>
    /// Règles logiques d'inférence.
    /// </summary>
    public DbSet<RegleDiagnostic> Regles => Set<RegleDiagnostic>();

    /// <summary>
    /// Sessions de diagnostic réalisées.
    /// </summary>
    public DbSet<SessionDiagnostic> Sessions => Set<SessionDiagnostic>();

    /// <summary>
    /// Symptômes associés aux sessions de diagnostic.
    /// </summary>
    public DbSet<SessionSymptome> SessionSymptomes => Set<SessionSymptome>();

    /// <summary>
    /// Résultats des tests enregistrés lors des sessions.
    /// </summary>
    public DbSet<SessionResultatTest> ResultatsTests => Set<SessionResultatTest>();

    /// <summary>
    /// Hypothèses de panne calculées pour les sessions.
    /// </summary>
    public DbSet<SessionHypothese> Hypotheses => Set<SessionHypothese>();

    /// <summary>
    /// Dossiers de réparation clôturés.
    /// </summary>
    public DbSet<DossierReparation> DossiersReparation => Set<DossierReparation>();

    /// <summary>
    /// Constructeur par défaut permettant l'instanciation sans conteneur IoC.
    /// </summary>
    public DiagDbContext()
    {
    }

    /// <summary>
    /// Constructeur permettant l'injection des options du contexte (ex. tests unitaires ou IoC).
    /// </summary>
    /// <param name="options">Options de configuration du DbContext.</param>
    public DiagDbContext(DbContextOptions<DiagDbContext> options)
        : base(options)
    {
    }

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string dbPath = GestionnaireSauvegarde.ObtenirCheminBase();
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- ModeleConsole ---
        modelBuilder.Entity<ModeleConsole>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ReferenceMaterielle).HasMaxLength(100);
            entity.HasIndex(e => e.Famille);
        });

        // --- Symptome ---
        modelBuilder.Entity<Symptome>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(300);
            entity.HasIndex(e => e.Categorie);
            entity.HasIndex(e => e.ModeleConsoleId);

            entity.HasOne(e => e.ModeleConsole)
                .WithMany()
                .HasForeignKey(e => e.ModeleConsoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // --- Panne ---
        modelBuilder.Entity<Panne>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).IsRequired();
            entity.HasIndex(e => e.ModeleConsoleId);

            entity.HasOne(e => e.ModeleConsole)
                .WithMany()
                .HasForeignKey(e => e.ModeleConsoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // --- PanneSymptome (clé composite) ---
        modelBuilder.Entity<PanneSymptome>(entity =>
        {
            entity.HasKey(ps => new { ps.PanneId, ps.SymptomeId });

            entity.HasOne(ps => ps.Panne)
                .WithMany(p => p.PanneSymptomes)
                .HasForeignKey(ps => ps.PanneId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ps => ps.Symptome)
                .WithMany(s => s.PanneSymptomes)
                .HasForeignKey(ps => ps.SymptomeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- TestDiagnostic ---
        modelBuilder.Entity<TestDiagnostic>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nom).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Objectif).IsRequired().HasMaxLength(300);
            entity.Property(e => e.PourquoiUtile).IsRequired();
            entity.Property(e => e.Etapes).IsRequired();
            entity.HasIndex(e => e.Difficulte);
            entity.HasIndex(e => e.ModeleConsoleId);

            entity.HasOne(e => e.ModeleConsole)
                .WithMany()
                .HasForeignKey(e => e.ModeleConsoleId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // --- RegleDiagnostic ---
        modelBuilder.Entity<RegleDiagnostic>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Source).HasMaxLength(200);
            entity.HasIndex(e => e.PanneId);
            entity.HasIndex(e => e.TestDiagnosticId);

            entity.HasOne(e => e.Panne)
                .WithMany(p => p.Regles)
                .HasForeignKey(e => e.PanneId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Test)
                .WithMany(t => t.Regles)
                .HasForeignKey(e => e.TestDiagnosticId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- SessionDiagnostic ---
        modelBuilder.Entity<SessionDiagnostic>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.NumeroSerie).HasMaxLength(50);
            entity.Property(e => e.EtatGeneral).HasMaxLength(300);
            entity.HasIndex(e => e.DateCreation);
            entity.HasIndex(e => e.Statut);

            entity.HasOne(e => e.ModeleConsole)
                .WithMany(m => m.Sessions)
                .HasForeignKey(e => e.ModeleConsoleId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.DossierReparation)
                .WithOne(d => d.SessionDiagnostic)
                .HasForeignKey<DossierReparation>(d => d.SessionDiagnosticId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // --- SessionSymptome (clé composite) ---
        modelBuilder.Entity<SessionSymptome>(entity =>
        {
            entity.HasKey(ss => new { ss.SessionDiagnosticId, ss.SymptomeId });

            entity.HasOne(ss => ss.SessionDiagnostic)
                .WithMany(s => s.SessionSymptomes)
                .HasForeignKey(ss => ss.SessionDiagnosticId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ss => ss.Symptome)
                .WithMany(s => s.SessionSymptomes)
                .HasForeignKey(ss => ss.SymptomeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- SessionResultatTest ---
        modelBuilder.Entity<SessionResultatTest>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SessionDiagnosticId);
            entity.HasIndex(e => e.TestDiagnosticId);
            entity.HasIndex(e => e.Horodatage);

            entity.HasOne(e => e.SessionDiagnostic)
                .WithMany(s => s.ResultatsTests)
                .HasForeignKey(e => e.SessionDiagnosticId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.TestDiagnostic)
                .WithMany(t => t.SessionResultats)
                .HasForeignKey(e => e.TestDiagnosticId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- SessionHypothese ---
        modelBuilder.Entity<SessionHypothese>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.SessionDiagnosticId);
            entity.HasIndex(e => e.PanneId);
            entity.HasIndex(e => e.Confiance);

            entity.HasOne(e => e.SessionDiagnostic)
                .WithMany(s => s.Hypotheses)
                .HasForeignKey(e => e.SessionDiagnosticId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Panne)
                .WithMany(p => p.Hypotheses)
                .HasForeignKey(e => e.PanneId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // --- DossierReparation ---
        modelBuilder.Entity<DossierReparation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ReferenceDossier).IsRequired().HasMaxLength(60);
            entity.HasIndex(e => e.ReferenceDossier).IsUnique();
            entity.HasIndex(e => e.DateDiagnostic);

            entity.HasOne(e => e.PanneIdentifiee)
                .WithMany(p => p.DossiersReparation)
                .HasForeignKey(e => e.PanneIdentifieeId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
