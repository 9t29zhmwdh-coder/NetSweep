using System.Text.Json.Serialization;

namespace NetSweep.Models;

/// <summary>
/// A saved connection to a network share (NAS / UNC path).
/// The password is stored DPAPI-encrypted (per Windows user) in EncryptedPassword.
/// </summary>
public class StorageConnection
{
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>Friendly display name shown in the connection list.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>UNC path, e.g. \\nas01\cleanup or a mapped folder.</summary>
    public string Path { get; set; } = string.Empty;

    /// <summary>Optional user name for the share (the restricted NAS account).</summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>DPAPI-encrypted password (base64). Never stored in clear text.</summary>
    public string EncryptedPassword { get; set; } = string.Empty;

    /// <summary>When set, deleted/cleaned files are moved here instead of removed.</summary>
    public string QuarantineFolder { get; set; } = string.Empty;

    /// <summary>Not persisted: live connection state for the current session.</summary>
    [JsonIgnore]
    public bool IsConnected { get; set; }

    public override string ToString() => string.IsNullOrWhiteSpace(Name) ? Path : Name;
}
