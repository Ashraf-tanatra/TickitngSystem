using ApplicationServices.DTOs.Account;
using Xunit;

namespace Test;

public sealed class DtoContractTests
{
    private static readonly HashSet<string> ForbiddenResponseProperties =
        new(StringComparer.OrdinalIgnoreCase)
        {
            "IsDeleted",
            "DeletedAt",
            "PasswordHash",
            "VerificationCode",
            "PasswordResetCode",
            "StoredFileName"
        };

    [Fact]
    public void ResponseDtosDoNotExposeInternalOrSensitiveProperties()
    {
        var responseTypes = typeof(LoginResponse).Assembly
            .GetTypes()
            .Where(type => type.Namespace?.Contains(".DTOs.", StringComparison.Ordinal) == true)
            .Where(type => type.Name.EndsWith("Response", StringComparison.Ordinal));

        var exposedProperties = responseTypes
            .SelectMany(type => type.GetProperties()
                .Where(property => ForbiddenResponseProperties.Contains(property.Name))
                .Select(property => $"{type.Name}.{property.Name}"))
            .ToArray();

        Assert.Empty(exposedProperties);
    }

    [Fact]
    public void LoginResponseDoesNotExposeIdsAlreadyStoredInTheToken()
    {
        var propertyNames = typeof(LoginResponse)
            .GetProperties()
            .Select(property => property.Name)
            .ToArray();

        Assert.DoesNotContain("AccountId", propertyNames);
        Assert.DoesNotContain("EmployeeId", propertyNames);
    }
}
