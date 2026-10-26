using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace XboxDiag.App.Converters;

/// <summary>
/// Convertisseur booléen vers visibilité WPF avec support d'inversion.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    public bool CollapseWhenFalse { get; set; } = true;

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool boolVal = false;
        if (value is bool b)
        {
            boolVal = b;
        }
        else if (value is int i)
        {
            boolVal = i > 0;
        }

        bool inverse = parameter is string str && str.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
        if (inverse)
        {
            boolVal = !boolVal;
        }

        if (boolVal)
        {
            return Visibility.Visible;
        }

        return CollapseWhenFalse ? Visibility.Collapsed : Visibility.Hidden;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is Visibility vis)
        {
            bool isVisible = vis == Visibility.Visible;
            bool inverse = parameter is string str && str.Equals("Inverse", StringComparison.OrdinalIgnoreCase);
            return inverse ? !isVisible : isVisible;
        }

        return false;
    }
}
