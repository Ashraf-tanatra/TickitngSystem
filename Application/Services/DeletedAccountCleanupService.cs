using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ApplicationServices.Services
{
    public class DeletedAccountCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DeletedAccountCleanupService(
            IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
    CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope =
                        _scopeFactory.CreateScope();

                    var context =
                        scope.ServiceProvider
                            .GetRequiredService<AppDbContext>();

                    var expirationDate =
                        DateTime.Now.AddDays(-30);

                    var accounts = await context.Accounts
                        .Where(a =>
                            a.IsDeleted &&
                            a.DeletedAt != null &&
                            a.DeletedAt <= expirationDate)
                        .ToListAsync(stoppingToken);

                    if (accounts.Any())
                    {
                        context.Accounts.RemoveRange(accounts);

                        await context.SaveChangesAsync(
                            stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Account cleanup error: {ex.Message}");
                }

                await Task.Delay(
                    TimeSpan.FromHours(24),
                    stoppingToken);
            }
        }
    }
}