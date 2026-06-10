namespace NetSweep.Helpers;

/// <summary>
/// Builds and parses UNC paths from the simple "address" + "share" parts the
/// user types in the connection dialog, so they never have to get the
/// backslashes right themselves.
/// </summary>
public static class PathBuilder
{
    /// <summary>Composes \\host\share from cleaned parts. Returns "" if host empty.</summary>
    public static string BuildUnc(string? host, string? share)
    {
        host = CleanHost(host);
        share = CleanShare(share);
        if (string.IsNullOrEmpty(host)) return string.Empty;
        return string.IsNullOrEmpty(share) ? $@"\\{host}" : $@"\\{host}\{share}";
    }

    /// <summary>Removes leading backslashes/slashes and surrounding spaces from an address.</summary>
    public static string CleanHost(string? host)
    {
        host = (host ?? string.Empty).Trim();
        host = host.Replace("/", "\\");
        host = host.TrimStart('\\');
        // If the user pasted a full UNC path into the address box, keep only the host.
        int slash = host.IndexOf('\\');
        if (slash >= 0) host = host[..slash];
        return host.Trim();
    }

    /// <summary>Normalises a share/sub-path: forward slashes to back, trims edge slashes.</summary>
    public static string CleanShare(string? share)
    {
        share = (share ?? string.Empty).Trim();
        share = share.Replace("/", "\\");
        return share.Trim('\\').Trim();
    }

    /// <summary>Splits a UNC path \\host\share\... into (host, share-rest).</summary>
    public static (string Host, string Share) ParseUnc(string? path)
    {
        path = (path ?? string.Empty).Trim();
        if (!path.StartsWith(@"\\")) return (string.Empty, string.Empty);
        string body = path[2..];
        int slash = body.IndexOf('\\');
        if (slash < 0) return (body, string.Empty);
        return (body[..slash], body[(slash + 1)..]);
    }
}
