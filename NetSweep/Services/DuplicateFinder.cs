using System.IO;
using System.Security.Cryptography;
using NetSweep.Models;

namespace NetSweep.Services;

/// <summary>
/// Finds duplicate files. Optimisation: only files with an identical size are
/// hashed (SHA-256), since files of different size can never be identical.
/// </summary>
public class DuplicateFinder
{
    public IProgress<string>? Progress { get; set; }

    public Task<List<DuplicateGroup>> FindAsync(IEnumerable<FileEntry> files, CancellationToken token)
        => Task.Run(() => Find(files, token), token);

    private List<DuplicateGroup> Find(IEnumerable<FileEntry> files, CancellationToken token)
    {
        var groups = new List<DuplicateGroup>();

        // Step 1: group by size, ignore singletons and zero-byte files.
        var bySize = files
            .Where(f => f.Size > 0)
            .GroupBy(f => f.Size)
            .Where(g => g.Count() > 1);

        foreach (var sizeGroup in bySize)
        {
            token.ThrowIfCancellationRequested();

            // Step 2: hash candidates and group by hash.
            var byHash = new Dictionary<string, DuplicateGroup>();
            foreach (var file in sizeGroup)
            {
                token.ThrowIfCancellationRequested();
                Progress?.Report(file.Name);
                string? hash = ComputeHash(file.FullPath);
                if (hash == null) continue;
                file.Hash = hash;

                if (!byHash.TryGetValue(hash, out var group))
                {
                    group = new DuplicateGroup { Hash = hash, Size = file.Size };
                    byHash[hash] = group;
                }
                group.Files.Add(file);
            }

            groups.AddRange(byHash.Values.Where(g => g.Files.Count > 1));
        }

        return groups.OrderByDescending(g => g.ReclaimableBytes).ToList();
    }

    private static string? ComputeHash(string path)
    {
        try
        {
            using var sha = SHA256.Create();
            using var stream = File.OpenRead(path);
            byte[] hash = sha.ComputeHash(stream);
            return Convert.ToHexString(hash);
        }
        catch
        {
            return null;
        }
    }
}
