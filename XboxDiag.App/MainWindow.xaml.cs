using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using XboxDiag.App.ViewModels;

namespace XboxDiag.App;

/// <summary>
/// Fenêtre principale de l'application XboxDiag.
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Résolution du MainViewModel via le conteneur IoC
        DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();
    }
}
