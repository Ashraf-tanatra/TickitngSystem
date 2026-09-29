using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;

namespace Test;

public sealed class SelfServiceRouteTests
{
    [Fact]
    public void RoutesDoNotAskForTheCurrentUsersIdentity()
    {
        var controllerTypes = new[]
        {
            typeof(Controller.AccountController),
            typeof(Controller.EmployeeController),
            typeof(Controller.ProjectController),
            typeof(Controller.TicketController)
        };

        var unnecessaryIdentityRoutes = controllerTypes
            .SelectMany(type => type.GetMethods())
            .SelectMany(method => method.GetCustomAttributes(typeof(HttpMethodAttribute), true)
                .Cast<HttpMethodAttribute>()
                .Select(attribute => $"{method.DeclaringType?.Name}.{method.Name}: {attribute.Template}"))
            .Where(route =>
                route.Contains("{employeeId", StringComparison.OrdinalIgnoreCase) ||
                route.Contains("{empId", StringComparison.OrdinalIgnoreCase) ||
                route.Contains("{accountId", StringComparison.OrdinalIgnoreCase) ||
                route.Contains("{email", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        Assert.Empty(unnecessaryIdentityRoutes);
    }
}
