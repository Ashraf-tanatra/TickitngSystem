namespace TickitngSystem.Security;

public static class JwtSigningKey
{
    private const int MinimumKeySizeInBytes = 32;

    public static byte[] Decode(string? encodedKey)
    {
        if (string.IsNullOrWhiteSpace(encodedKey))
            throw new InvalidOperationException(
                "Jwt:Key is required and must be stored outside source control.");

        byte[] keyBytes;
        try
        {
            keyBytes = Convert.FromBase64String(encodedKey);
        }
        catch (FormatException ex)
        {
            throw new InvalidOperationException(
                "Jwt:Key must be a valid Base64-encoded secret.",
                ex);
        }

        if (keyBytes.Length < MinimumKeySizeInBytes)
            throw new InvalidOperationException(
                "Jwt:Key must decode to at least 32 random bytes (256 bits).");

        return keyBytes;
    }
}
