using System.Globalization;

namespace NetSweep.Helpers;

/// <summary>Formats byte counts as human readable strings.</summary>
public static class ByteSize
{
    private static readonly string[] Units = { "B", "KB", "MB", "GB", "TB", "PB" };

    public static string Format(long bytes)
    {
        if (bytes < 0) return "-" + Format(-bytes);
        double value = bytes;
        int unit = 0;
        while (value >= 1024 && unit < Units.Length - 1)
        {
            value /= 1024;
            unit++;
        }
        string format = unit == 0 ? "0" : "0.##";
        return value.ToString(format, CultureInfo.CurrentCulture) + " " + Units[unit];
    }

    /// <summary>Parses values like "500 MB", "2gb", "1024". Returns bytes or null.</summary>
    public static long? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        text = text.Trim().ToUpperInvariant();
        double multiplier = 1;
        foreach (var (suffix, factor) in new[]
        {
            ("PB", 1024d * 1024 * 1024 * 1024 * 1024),
            ("TB", 1024d * 1024 * 1024 * 1024),
            ("GB", 1024d * 1024 * 1024),
            ("MB", 1024d * 1024),
            ("KB", 1024d),
            ("B", 1d),
        })
        {
            if (text.EndsWith(suffix))
            {
                multiplier = factor;
                text = text[..^suffix.Length].Trim();
                break;
            }
        }
        if (double.TryParse(text, NumberStyles.Any, CultureInfo.CurrentCulture, out var n) ||
            double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out n))
        {
            return (long)(n * multiplier);
        }
        return null;
    }
}
