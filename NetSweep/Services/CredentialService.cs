using System.Security.Cryptography;
using System.Text;

namespace NetSweep.Services;

/// <summary>
/// Encrypts/decrypts secrets with Windows DPAPI, scoped to the current user.
/// The ciphertext can only be decrypted by the same Windows account on the same
/// machine, so stored NAS passwords never leave the user's profile in clear text.
/// </summary>
public static class CredentialService
{
    private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("NetSweep.v1.entropy");

    public static string Encrypt(string? plainText)
    {
        if (string.IsNullOrEmpty(plainText)) return string.Empty;
        byte[] data = Encoding.UTF8.GetBytes(plainText);
        byte[] encrypted = ProtectedData.Protect(data, Entropy, DataProtectionScope.CurrentUser);
        return Convert.ToBase64String(encrypted);
    }

    public static string Decrypt(string? cipherText)
    {
        if (string.IsNullOrEmpty(cipherText)) return string.Empty;
        try
        {
            byte[] encrypted = Convert.FromBase64String(cipherText);
            byte[] data = ProtectedData.Unprotect(encrypted, Entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(data);
        }
        catch
        {
            // Wrong machine/user or corrupted blob: treat as no password.
            return string.Empty;
        }
    }
}
