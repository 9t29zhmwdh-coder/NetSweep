using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using NetSweep.Helpers;

namespace NetSweep.ViewModels;

/// <summary>Shows the localized "Connected" / "Disconnected" text for a bool.</summary>
public class BoolToStatusConverter : IValueConverter
{
    public static readonly BoolToStatusConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Loc.Instance.Get(value is true ? "Connected" : "Disconnected");

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Shows the localized " (N files)" suffix for a folder's file count.</summary>
public class FileCountToTextConverter : IValueConverter
{
    public static readonly FileCountToTextConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => Loc.Instance.Get("FileCountSuffix", value ?? 0);

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}

/// <summary>Green when connected, grey when not.</summary>
public class BoolToBrushConverter : IValueConverter
{
    public static readonly BoolToBrushConverter Instance = new();
    private static readonly Brush Connected = new SolidColorBrush(Color.FromRgb(0x2E, 0x7D, 0x32));
    private static readonly Brush Disconnected = new SolidColorBrush(Color.FromRgb(0x9E, 0x9E, 0x9E));

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? Connected : Disconnected;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
