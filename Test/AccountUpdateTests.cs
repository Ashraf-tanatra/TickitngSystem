using ApplicationServices.DTOs.Account;
using ApplicationServices.Interfaces;
using ApplicationServices.Services;
using Domain.Entities;
using Domain.Enum;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Test;

public sealed class AccountUpdateTests
{
    [Fact]
    public async Task ChangingEmailRequiresVerificationOfNewAddress()
    {
        await using var context = CreateContext();
        const string password = "Valid1@Password";
        var employee = Employee.Create("Test", "User", "+970590000005", Gender.M);
        var account = Account.Create(
            "old@example.com",
            BCrypt.Net.BCrypt.HashPassword(password),
            employee);
        account.VerifyEmail();

        context.Add(employee);
        await context.SaveChangesAsync();

        var emailService = new RecordingEmailService();
        var manager = new AccountManager(
            new AccountRepository(context),
            new EmployeeRepository(context),
            emailService);

        await manager.UpdateAsync(account.Id, new UpdateAccountRequest
        {
            Email = "new@example.com",
            CurrentPassword = password
        });

        Assert.False(account.IsEmailVerified);
        Assert.NotNull(account.VerificationCode);
        Assert.Equal("new@example.com", emailService.Email);
        Assert.Equal(account.VerificationCode, emailService.Code);
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    private sealed class RecordingEmailService : IEmailService
    {
        public string? Email { get; private set; }
        public string? Code { get; private set; }

        public Task SendVerificationCodeAsync(string email, string code)
        {
            Email = email;
            Code = code;
            return Task.CompletedTask;
        }
    }
}
