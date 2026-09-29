using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Xunit;

namespace Test;

public sealed class DashboardRouteTests
{
    [Fact]
    public void DashboardUsesOneEndpoint()
    {
        var endpoint = typeof(Controller.DashboardController)
            .GetMethod(nameof(Controller.DashboardController.GetMyDashboard));

        Assert.NotNull(endpoint);
        var route = Assert.Single(endpoint
            .GetCustomAttributes(typeof(HttpGetAttribute), true)
            .Cast<HttpGetAttribute>());
        Assert.Equal("me", route.Template);
    }

    [Fact]
    public void OldDashboardEndpointsAreRemoved()
    {
        var oldRoutes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "me/count",
            "me/counts",
            "me/completed-count",
            "me/dashboard/top-three",
            "me/dashboard/projects",
            "me/recent-activity"
        };

        var remainingOldRoutes = new[]
            {
                typeof(Controller.ProjectController),
                typeof(Controller.TicketController)
            }
            .SelectMany(type => type.GetMethods())
            .SelectMany(method => method
                .GetCustomAttributes(typeof(HttpMethodAttribute), true)
                .Cast<HttpMethodAttribute>())
            .Select(attribute => attribute.Template)
            .Where(template => template != null && oldRoutes.Contains(template))
            .ToArray();

        Assert.Empty(remainingOldRoutes);
    }
}
