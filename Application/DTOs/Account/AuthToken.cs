namespace ApplicationServices.DTOs.Account;

public sealed record AuthToken(string Value, DateTime ExpiresAtUtc);
