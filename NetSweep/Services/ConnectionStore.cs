using System.IO;
using System.Text.Json;
using NetSweep.Models;

namespace NetSweep.Services;

/// <summary>
/// Loads and saves the list of saved connections as JSON under
/// %AppData%\NetSweep\connections.json. Passwords are already DPAPI-encrypted
/// inside each StorageConnection.
/// </summary>
public class ConnectionStore
{
    private static readonly string Dir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NetSweep");

    private static readonly string FilePath = Path.Combine(Dir, "connections.json");

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public List<StorageConnection> Load()
    {
        try
        {
            if (!File.Exists(FilePath)) return new List<StorageConnection>();
            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<List<StorageConnection>>(json) ?? new List<StorageConnection>();
        }
        catch
        {
            return new List<StorageConnection>();
        }
    }

    public void Save(IEnumerable<StorageConnection> connections)
    {
        Directory.CreateDirectory(Dir);
        string json = JsonSerializer.Serialize(connections, JsonOptions);
        File.WriteAllText(FilePath, json);
    }
}
