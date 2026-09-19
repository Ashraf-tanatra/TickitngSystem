using Domain.Entities;
using Domain.Enum;
using Xunit;

namespace Test;

public sealed class DomainValidationTests
{
    [Fact]
    public void EmployeeCreateRejectsUnknownGender()
    {
        Assert.Throws<ArgumentException>(() =>
            Employee.Create("Test", "User", "+970590000000", (Gender)99));
    }

    [Fact]
    public void ProjectCreateRejectsEndDateBeforeStartDate()
    {
        Assert.Throws<ArgumentException>(() =>
            Project.Create(
                "Project",
                null,
                Guid.NewGuid(),
                new DateOnly(2026, 9, 20),
                new DateOnly(2026, 9, 19)));
    }

    [Fact]
    public void TicketCreateRejectsUnknownPriority()
    {
        Assert.Throws<ArgumentException>(() =>
            Ticket.Create(
                "Ticket",
                null,
                null,
                (TicketPriority)99,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()));
    }

    [Fact]
    public void TicketCreateRejectsTitleLongerThanBusinessLimit()
    {
        Assert.Throws<ArgumentException>(() =>
            Ticket.Create(
                new string('x', ErrorShared.Ticket.TicketTitleMaxLength + 1),
                null,
                null,
                TicketPriority.Medium,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid()));
    }
}
