using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using XboxDiag.Core.Models;

namespace XboxDiag.App.Converters;

/// <summary>
/// Convertisseur associant un statut d'hypothèse à une couleur Brush correspondante.
/// </summary>
public class StatutToColorConverter : IValueConverter
{
    private static readonly SolidColorBrush GrayBrush = new((Color)ColorConverter.ConvertFromString("#71717A"));
    private static readonly SolidColorBrush DodgerBlueBrush = new((Color)ColorConverter.ConvertFromString("#1E90FF"));
    private static readonly SolidColorBrush OrangeBrush = new((Color)ColorConverter.ConvertFromString("#F59E0B"));
    private static readonly SolidColorBrush LightGrayBrush = new((Color)ColorConverter.ConvertFromString("#A1A1AA"));
    private static readonly SolidColorBrush RedBrush = new((Color)ColorConverter.ConvertFromString("#EF4444"));
    private static readonly SolidColorBrush GreenBrush = new((Color)ColorConverter.ConvertFromString("#107C10"));
    private static readonly SolidColorBrush PurpleBrush = new((Color)ColorConverter.ConvertFromString("#8B5CF6"));

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is StatutHypothese statut)
        {
            return statut switch
            {
                StatutHypothese.NonVerifiee => GrayBrush,
                StatutHypothese.CompatibleSymptomes => DodgerBlueBrush,
                StatutHypothese.RenforceeParTests => OrangeBrush,
                StatutHypothese.PeuProbable => LightGrayBrush,
                StatutHypothese.EcarteeParTest => RedBrush,
                StatutHypothese.ConfirmeeParVerification => GreenBrush,
                StatutHypothese.ImpossibleADepartager => PurpleBrush,
                _ => LightGrayBrush
            };
        }

        return LightGrayBrush;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
