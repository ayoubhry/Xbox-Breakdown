using System;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using QuestPDF.Infrastructure;
using XboxDiag.App.Services;
using XboxDiag.App.ViewModels;
using XboxDiag.Core.Data;
using XboxDiag.Core.Engine;
using XboxDiag.Core.Reports;

namespace XboxDiag.App;

/// <summary>
/// Point d'entrée principal de l'application WPF XboxDiag.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Fournisseur de services d'injection de dépendances accessible globalement.
    /// </summary>
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    public App()
    {
        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            LoguerErreur(args.ExceptionObject as Exception, "AppDomain.CurrentDomain");
        };

        DispatcherUnhandledException += (s, args) =>
        {
            LoguerErreur(args.Exception, "DispatcherUnhandledException");
            MessageBox.Show(
                $"Une anomalie inattendue s'est produite :\n\n{args.Exception?.Message}\n\nUn rapport détaillé a été consigné dans crash.log.",
                "Erreur de l'application Xbox Breakdown",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
            args.Handled = true;
        };

        System.Threading.Tasks.TaskScheduler.UnobservedTaskException += (s, args) =>
        {
            LoguerErreur(args.Exception, "TaskScheduler.UnobservedTaskException");
            args.SetObserved();
        };
    }

    private static void LoguerErreur(Exception? ex, string source)
    {
        try
        {
            string cheminLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "crash.log");
            string contenu = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{source}]\n{ex?.ToString()}\n--------------------------------------------------\n";
            File.AppendAllText(cheminLog, contenu);
        }
        catch
        {
            // Ignorer si échec d'écriture dans le journal
        }
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // 1. Activation de la licence communautaire QuestPDF
        QuestPDF.Settings.License = LicenseType.Community;

        // 2. Configuration du conteneur de dépendances (IoC)
        var services = new ServiceCollection();

        // Base de données et Moteur expert
        services.AddDbContext<DiagDbContext>();
        services.AddScoped<IDiagnosticEngine, DiagnosticEngine>();
        services.AddSingleton<GenerateurRapport>();

        // Navigation
        services.AddSingleton<NavigationService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();
        services.AddTransient<AccueilViewModel>();
        services.AddTransient<IdentificationConsoleViewModel>();
        services.AddTransient<SelectionSymptomesViewModel>();
        services.AddTransient<DiagnosticViewModel>();
        services.AddTransient<AssistantTestViewModel>();
        services.AddTransient<RapportViewModel>();
        services.AddTransient<HistoriqueViewModel>();
        services.AddTransient<BaseConnaissancesViewModel>();

        ServiceProvider = services.BuildServiceProvider();

        // 3. Initialisation et amorçage des données SQLite (SeedData)
        using (var scope = ServiceProvider.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DiagDbContext>();
            SeedData.Initialiser(dbContext);
        }
    }
}
