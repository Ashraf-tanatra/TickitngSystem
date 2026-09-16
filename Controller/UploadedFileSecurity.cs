using Microsoft.AspNetCore.Http;

namespace Controller;

internal static class UploadedFileSecurity
{
    private static readonly byte[] JpegSignature = [0xFF, 0xD8, 0xFF];
    private static readonly byte[] PngSignature = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
    private static readonly byte[] Gif87Signature = "GIF87a"u8.ToArray();
    private static readonly byte[] Gif89Signature = "GIF89a"u8.ToArray();

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
