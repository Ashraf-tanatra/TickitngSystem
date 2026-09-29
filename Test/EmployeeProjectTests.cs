using ApplicationServices.Services;
using Domain.Entities;
using Domain.Enum;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test;

public sealed class EmployeeProjectTests
{
    [Fact]
    public async Task GetProjectsIncludesProjectsManagedByEmployee()
    {
        await using var context = CreateContext();
        var manager = Employee.Create("Manager", "User", "+970590000001", Gender.M);
        var member = Employee.Create("Member", "User", "+970590000002", Gender.F);
        var project = Project.Create("Managed project", null, manager.Id, null, null);
        var membership = ProjectEmployee.Create(project.Id, member.Id, "Developer");

        context.AddRange(manager, member, project, membership);
        await context.SaveChangesAsync();

        var repository = new EmployeeRepository(context);
        var projects = await repository.GetProjectsAsync(manager.Id);

        Assert.Contains(projects, item => item.Id == project.Id);
    }

    [Fact]
    public async Task ManagerProjectResponseUsesManagerRoleAndCountsManager()
    {
        await using var context = CreateContext();
        var manager = Employee.Create("Manager", "User", "+970590000003", Gender.M);
        var member = Employee.Create("Member", "User", "+970590000004", Gender.F);
        var project = Project.Create("Managed project", null, manager.Id, null, null);
        var membership = ProjectEmployee.Create(project.Id, member.Id, "Developer");

        context.AddRange(manager, member, project, membership);
        await context.SaveChangesAsync();

        var employeeManager = new EmployeeManager(
            new EmployeeRepository(context),
            new AccountRepository(context));

        var response = Assert.Single(await employeeManager.GetProjectsAsync(manager.Id));

        Assert.Equal(ErrorShared.Project.ManagerRole, response.Role);
        Assert.Equal(2, response.EmployeeCount);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
