using System.Security.Claims;

namespace Controller;

internal static class ClaimsPrincipalExtensions
{
    public static Guid GetEmployeeId(this ClaimsPrincipal user) =>
        GetGuidClaim(user, "employee_id");

    public static Guid GetAccountId(this ClaimsPrincipal user) =>
        GetGuidClaim(user, "account_id");

    public static string GetEmail(this ClaimsPrincipal user) =>
        user.FindFirstValue(ClaimTypes.Email)
        ?? user.FindFirstValue("email")
        ?? throw new UnauthorizedAccessException("The access token does not contain an email address.");

    private static Guid GetGuidClaim(ClaimsPrincipal user, string claimType)
    {
        var value = user.FindFirstValue(claimType);
        if (!Guid.TryParse(value, out var id) || id == Guid.Empty)
            throw new UnauthorizedAccessException($"The access token does not contain a valid {claimType} claim.");

        return id;
    }
}
