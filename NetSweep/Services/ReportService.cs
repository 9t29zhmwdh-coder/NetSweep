using System.Globalization;
using System.IO;
using System.Text;
using NetSweep.Models;

namespace NetSweep.Services;

/// <summary>Exports scan results to a CSV file (semicolon-separated for Excel CH/DE).</summary>
public static class ReportService
{
    public static void ExportFiles(IEnumerable<FileEntry> files, string targetPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Name;Pfad;Groesse (Bytes);Groesse;Geaendert;Letzter Zugriff;Alter (Tage);Typ");
        foreach (var f in files)
        {
            sb.AppendLine(string.Join(';',
                Csv(f.Name),
                Csv(f.FullPath),
                f.Size.ToString(CultureInfo.InvariantCulture),
                Csv(f.SizeText),
                f.LastModified.ToString("yyyy-MM-dd HH:mm"),
                f.LastAccessed.ToString("yyyy-MM-dd HH:mm"),
                f.AgeDays.ToString(CultureInfo.InvariantCulture),
                Csv(f.Extension)));
        }
        File.WriteAllText(targetPath, sb.ToString(), new UTF8Encoding(true));
    }

    public static void ExportDuplicates(IEnumerable<DuplicateGroup> groups, string targetPath)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Hash;Groesse;Anzahl;Wiedergewinnbar;Datei");
        foreach (var g in groups)
        {
            foreach (var f in g.Files)
            {
                sb.AppendLine(string.Join(';',
                    Csv(g.Hash[..Math.Min(12, g.Hash.Length)]),
                    Csv(Helpers.ByteSize.Format(g.Size)),
                    g.Files.Count.ToString(CultureInfo.InvariantCulture),
                    Csv(g.ReclaimableText),
                    Csv(f.FullPath)));
            }
        }
        File.WriteAllText(targetPath, sb.ToString(), new UTF8Encoding(true));
    }

    private static string Csv(string? value)
    {
        value ??= string.Empty;
        if (value.Contains(';') || value.Contains('"') || value.Contains('\n'))
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }
}
