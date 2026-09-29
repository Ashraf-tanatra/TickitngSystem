using Microsoft.AspNetCore.Http;
using System.IO.Compression;
using System.Text;

namespace Controller;

internal static class UploadedFileSecurity
{
    private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] Gif87Signature = "GIF87a"u8.ToArray();
    private static readonly byte[] Gif89Signature = "GIF89a"u8.ToArray();
    private static readonly byte[] PdfSignature = "%PDF-"u8.ToArray();
    private static readonly UTF8Encoding StrictUtf8 = new(false, true);

    private static readonly IReadOnlyDictionary<string, string[]> AllowedAttachmentTypes =
        new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase)
        {
            [".pdf"] = ["application/pdf"],
            [".jpg"] = ["image/jpeg"],
            [".jpeg"] = ["image/jpeg"],
            [".png"] = ["image/png"],
            [".gif"] = ["image/gif"],
            [".webp"] = ["image/webp"],
            [".txt"] = ["text/plain"],
            [".csv"] = ["text/csv"],
            [".docx"] = ["application/vnd.openxmlformats-officedocument.wordprocessingml.document"],
            [".xlsx"] = ["application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"]
        };

    public static string AllowedAttachmentExtensions =>
        string.Join(", ", AllowedAttachmentTypes.Keys.OrderBy(extension => extension));

    public static async Task<bool> HasValidImageSignatureAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header, cancellationToken);
        var bytes = header.AsSpan(0, bytesRead);

        return file.ContentType.ToLowerInvariant() switch
        {
            "image/jpeg" => bytes.StartsWith(JpegSignature),
            "image/png" => bytes.StartsWith(PngSignature),
            "image/gif" => bytes.StartsWith(Gif87Signature) || bytes.StartsWith(Gif89Signature),
            "image/webp" => bytesRead >= 12 &&
                bytes[..4].SequenceEqual("RIFF"u8) &&
                bytes[8..12].SequenceEqual("WEBP"u8),
            _ => false
        };
    }

    public static async Task<bool> IsSafeAttachmentAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(Path.GetFileName(file.FileName));
        if (!AllowedAttachmentTypes.TryGetValue(extension, out var allowedContentTypes) ||
            !allowedContentTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
        {
            return false;
        }

        if (file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
            return await HasValidImageSignatureAsync(file, cancellationToken);

        if (extension.Equals(".pdf", StringComparison.OrdinalIgnoreCase))
            return await StartsWithAsync(file, PdfSignature, cancellationToken);

        if (extension.Equals(".docx", StringComparison.OrdinalIgnoreCase))
            return HasRequiredZipEntry(file, "word/document.xml");

        if (extension.Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            return HasRequiredZipEntry(file, "xl/workbook.xml");

        return await IsUtf8TextAsync(file, cancellationToken);
    }

    private static async Task<bool> StartsWithAsync(
        IFormFile file,
        byte[] signature,
        CancellationToken cancellationToken)
    {
        var header = new byte[signature.Length];
        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header, cancellationToken);
        return bytesRead == signature.Length && header.AsSpan().SequenceEqual(signature);
    }

    private static bool HasRequiredZipEntry(IFormFile file, string requiredEntry)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var archive = new ZipArchive(stream, ZipArchiveMode.Read, leaveOpen: false);
            var containsUnsafeEmbeddedContent = archive.Entries.Any(entry =>
                entry.FullName.EndsWith("vbaProject.bin", StringComparison.OrdinalIgnoreCase) ||
                entry.FullName.Contains("/embeddings/", StringComparison.OrdinalIgnoreCase));

            return !containsUnsafeEmbeddedContent &&
                archive.GetEntry("[Content_Types].xml") != null &&
                archive.GetEntry(requiredEntry) != null;
        }
        catch (InvalidDataException)
        {
            return false;
        }
    }

    private static async Task<bool> IsUtf8TextAsync(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = file.OpenReadStream();
            using var reader = new StreamReader(
                stream,
                StrictUtf8,
                detectEncodingFromByteOrderMarks: true,
                leaveOpen: false);
            var buffer = new char[4096];
            int charactersRead;
            while ((charactersRead = await reader.ReadAsync(buffer, cancellationToken)) > 0)
            {
                if (buffer.AsSpan(0, charactersRead).Contains('\0'))
                    return false;
            }

            return true;
        }
        catch (DecoderFallbackException)
        {
            return false;
        }
    }

    public static string CreateStoredFileName(string originalFileName)
    {
        var extension = Path.GetExtension(Path.GetFileName(originalFileName)).ToLowerInvariant();
        var safeExtension = extension.Length is > 1 and <= 11 &&
            extension[1..].All(char.IsAsciiLetterOrDigit)
                ? extension
                : string.Empty;

        return $"{Guid.NewGuid():N}{safeExtension}";
    }
}
