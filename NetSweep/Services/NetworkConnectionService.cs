using System.IO;
using System.Runtime.InteropServices;
using NetSweep.Helpers;
using NetSweep.Models;

namespace NetSweep.Services;

/// <summary>
/// Establishes an authenticated session to a UNC share using the Windows
/// WNetAddConnection2 API. This lets us access \\nas\share with the restricted
/// NAS account without mapping a drive letter.
/// </summary>
public class NetworkConnectionService
{
    [StructLayout(LayoutKind.Sequential)]
    private class NetResource
    {
        public int Scope;
        public int Type;
        public int DisplayType;
        public int Usage;
        public string? LocalName;
        public string? RemoteName;
        public string? Comment;
        public string? Provider;
    }

    private const int ResourcetypeDisk = 0x00000001;
    private const int ConnectInteractive = 0x00000008;

    [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
    private static extern int WNetAddConnection2(NetResource netResource, string? password, string? username, int flags);

    [DllImport("mpr.dll", CharSet = CharSet.Unicode)]
    private static extern int WNetCancelConnection2(string name, int flags, bool force);

    /// <summary>
    /// Connects to the share. Returns (success, message). If no username is given
    /// the current Windows identity is used. If the share is already reachable
    /// this still succeeds.
    /// </summary>
    public (bool Success, string Message) Connect(StorageConnection connection)
    {
        string path = connection.Path?.TrimEnd('\\') ?? string.Empty;
        if (string.IsNullOrWhiteSpace(path))
            return (false, Localization.Instance.Get("NoPathGiven"));

        // Local folders need no authentication.
        if (!path.StartsWith(@"\\"))
        {
            bool exists = Directory.Exists(path);
            connection.IsConnected = exists;
            return exists
                ? (true, Localization.Instance.Get("ConnectedLocalPath"))
                : (false, Localization.Instance.Get("PathNotFound"));
        }

        var resource = new NetResource
        {
            Scope = 2,
            Type = ResourcetypeDisk,
            DisplayType = 3,
            Usage = 1,
            RemoteName = path
        };

        string? password = string.IsNullOrEmpty(connection.Username)
            ? null
            : CredentialService.Decrypt(connection.EncryptedPassword);
        string? username = string.IsNullOrEmpty(connection.Username) ? null : connection.Username;

        int result = WNetAddConnection2(resource, password, username, ConnectInteractive);

        // 0 = OK, 1219 = already connected with different creds, 85 = already in use
        if (result == 0 || result == 1219 || result == 85)
        {
            connection.IsConnected = Directory.Exists(path);
            return connection.IsConnected
                ? (true, Localization.Instance.Get("ConnectionEstablished"))
                : (false, Localization.Instance.Get("AuthenticatedPathUnreachable", result));
        }

        connection.IsConnected = false;
        return (false, DescribeError(result));
    }

    public void Disconnect(StorageConnection connection)
    {
        string path = connection.Path?.TrimEnd('\\') ?? string.Empty;
        if (path.StartsWith(@"\\"))
        {
            WNetCancelConnection2(path, 0, true);
        }
        connection.IsConnected = false;
    }

    private static string DescribeError(int code) => code switch
    {
        5 => Localization.Instance.Get("AccessDenied"),
        53 => Localization.Instance.Get("NetworkPathNotFound"),
        67 => Localization.Instance.Get("NetworkNameNotFound"),
        86 => Localization.Instance.Get("WrongPassword"),
        1326 => Localization.Instance.Get("LoginFailed"),
        _ => Localization.Instance.Get("ConnectionFailedCode", code)
    };
}
