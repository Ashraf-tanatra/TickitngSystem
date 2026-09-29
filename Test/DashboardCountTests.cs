using Domain.Entities;
using Domain.Enum;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test;

public sealed class DashboardCountTests
{
    [Fact]
    public async Task TicketCountsPreserveExistingStatusRules()
    {
        await using var context = CreateContext();
        var employee = Employee.Create("Dashboard", "User", "+970590000010", Gender.M);
        var project = Project.Create("Dashboard project", null, employee.Id, null, null);

        var tickets = new[]
        {
            CreateTicket(project.Id, employee.Id, TicketStatus.Pending),
            CreateTicket(project.Id, employee.Id, TicketStatus.InProgress),
            CreateTicket(project.Id, employee.Id, TicketStatus.NeedReview),
            CreateTicket(project.Id, employee.Id, TicketStatus.InReview),
            CreateTicket(project.Id, employee.Id, TicketStatus.Completed),
            CreateTicket(project.Id, employee.Id, TicketStatus.Done),
            CreateTicket(project.Id, employee.Id, TicketStatus.Cancelled)
        };

        context.AddRange(employee, project);
        context.Tickets.AddRange(tickets);
        await context.SaveChangesAsync();

        var repository = new TicketRepository(context);
        var counts = await repository.GetTicketCountsForAnEmployeeAsync(employee.Id);
        var completedCount = await repository.GetTicketCompletedCountForAnEmployeeAsync(employee.Id);
        var activeTickets = await repository.GetAllTicketsForAnEmployeeAsync(employee.Id);

        Assert.Equal(5, counts.TicketCount);
        Assert.Equal(1, counts.InProgressCount);
        Assert.Equal(2, counts.NeedReviewCount);
        Assert.Equal(1, completedCount);
        Assert.Equal(5, activeTickets.Count());
    }

    [Fact]
    public async Task RecentTicketsReturnThreeTicketsOrderedByLatestActivity()
    {
        await using var context = CreateContext();
        var employee = Employee.Create("Recent", "User", "+970590000011", Gender.M);
        var project = Project.Create("Recent project", null, employee.Id, null, null);
        var tickets = Enumerable.Range(1, 4)
            .Select(index => Ticket.Create(
                $"Ticket {index}",
                null,
                null,
                TicketPriority.Medium,
                project.Id,
                employee.Id,
                employee.Id))
            .ToArray();

        context.AddRange(employee, project);
        context.Tickets.AddRange(tickets);
        await context.SaveChangesAsync();

        var now = DateTime.Now;
        for (var index = 0; index < tickets.Length; index++)
        {
            context.Entry(tickets[index]).Property(ticket => ticket.CreatedAt).CurrentValue =
                now.AddHours(-(index + 1));
        }

        var history = TicketHistory.Create(
            tickets[3].TicketId,
            employee.Id,
            "Comment");
        context.Add(history);
        await context.SaveChangesAsync();
        context.Entry(history).Property(item => item.ModifiedAt).CurrentValue = now.AddMinutes(-1);
        await context.SaveChangesAsync();

        var repository = new TicketRepository(context);
        var recentTickets = (await repository
                .GetRecentTicketsWithActivityAsync(employee.Id))
            .ToArray();

        Assert.Equal(3, recentTickets.Length);
        Assert.Equal(tickets[3].TicketId, recentTickets[0].TicketId);
        Assert.Equal(tickets[0].TicketId, recentTickets[1].TicketId);
        Assert.Equal(tickets[1].TicketId, recentTickets[2].TicketId);
    }

    private static Ticket CreateTicket(
        Guid projectId,
        Guid employeeId,
        TicketStatus status)
    {
        var ticket = Ticket.Create(
            $"Ticket {status}",
            null,
            null,
            TicketPriority.Medium,
            projectId,
            employeeId,
            employeeId);
        ticket.ChangeStatus(status);
        return ticket;
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
