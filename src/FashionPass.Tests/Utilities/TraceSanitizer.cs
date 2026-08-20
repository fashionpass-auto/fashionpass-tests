using System.IO.Compression;
using System.Text;
using FashionPass.Tests.Config;

namespace FashionPass.Tests.Utilities;

public static class TraceSanitizer
{
    private const string Redaction = "[REDACTED]";

    private static readonly string[] TextEntrySuffixes =
    {
        ".trace", ".network", ".stacks", ".html", ".txt", ".json", ".css", ".js"
    };

    public static string Sanitize(string sourceZip, TestConfig config)
    {
        var secrets = CollectSecrets(config);
        if (secrets.Count == 0)
            return sourceZip;

        var sanitizedPath = Path.ChangeExtension(sourceZip, ".sanitized.zip");

        using var source = ZipFile.OpenRead(sourceZip);
        using var destination = ZipFile.Open(sanitizedPath, ZipArchiveMode.Create);

        foreach (var entry in source.Entries)
        {
            var data = ReadAllBytes(entry);

            if (IsTextEntry(entry.Name))
            {
                var text = Encoding.UTF8.GetString(data);
                foreach (var secret in secrets)
                    text = text.Replace(secret, Redaction);
                data = Encoding.UTF8.GetBytes(text);
            }

            var newEntry = destination.CreateEntry(entry.FullName, CompressionLevel.Optimal);
            using var stream = newEntry.Open();
            stream.Write(data);
        }

        return sanitizedPath;
    }

    private static List<string> CollectSecrets(TestConfig config)
    {
        var secrets = new List<string>();
        var user = config.Users.Default;
        Add(secrets, user.Email);
        Add(secrets, user.Password);
        Add(secrets, user.FirstName);
        Add(secrets, user.LastName);
        Add(secrets, user.Phone);
        Add(secrets, config.Email.Password);
        return secrets;
    }

    private static void Add(List<string> secrets, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value) && value.Length >= 4)
            secrets.Add(value);
    }

    private static bool IsTextEntry(string entryName)
    {
        foreach (var suffix in TextEntrySuffixes)
        {
            if (entryName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
                return true;
        }

        return false;
    }

    private static byte[] ReadAllBytes(ZipArchiveEntry entry)
    {
        using var stream = entry.Open();
        using var memory = new MemoryStream();
        stream.CopyTo(memory);
        return memory.ToArray();
    }
}