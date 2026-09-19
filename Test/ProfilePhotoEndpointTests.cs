using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Xunit;

namespace Test;

public sealed class ProfilePhotoEndpointTests
{
    [Fact]
    public void ProfilePhotoCanBeRenderedWithoutAuthorizationHeader()
    {
        var endpoint = typeof(Controller.EmployeeController)
            .GetMethod(nameof(Controller.EmployeeController.GetProfilePhoto));

        Assert.NotNull(endpoint);
        Assert.NotNull(endpoint.GetCustomAttribute<AllowAnonymousAttribute>());
    }
}
