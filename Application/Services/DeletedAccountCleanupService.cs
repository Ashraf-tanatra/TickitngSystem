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

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
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

                    var expirationDate = DateTime.UtcNow.AddDays(
                        -ErrorShared.Account.ReactivationPeriodDays);

                    var accounts = await context.Accounts
                        .Include(a => a.Employee)
                        .Where(a =>
                            a.IsDeleted &&
                            !a.IsAnonymized &&
                            a.DeletedAt != null &&
                            a.DeletedAt <= expirationDate)
                        .ToListAsync(stoppingToken);

                    foreach (var account in accounts)
                    {
                        var profileImageUrl = account.Employee.ProfileImageUrl;
                        var anonymousEmail =
                            $"deleted-{account.Id}-{Guid.NewGuid():N}@deleted.invalid";
                        var unusablePasswordHash = BCrypt.Net.BCrypt.HashPassword(
                            Guid.NewGuid().ToString("N"));

                        account.Anonymize(
                            anonymousEmail,
                            unusablePasswordHash);

                        if (!account.Employee.IsDeleted)
                            account.Employee.Deactivate();

                        account.Employee.Anonymize();
                        DeleteProfileImage(profileImageUrl);
                    }

                    if (accounts.Any())
                    {
                        await context.SaveChangesAsync(
                            stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"{ErrorShared.System.CleanupErrorPrefix}{ex.Message}");
                }

                await Task.Delay(
                    TimeSpan.FromHours(24),
                    stoppingToken);
            }
        }

        private static void DeleteProfileImage(string? profileImageUrl)
        {
            if (string.IsNullOrWhiteSpace(profileImageUrl))
                return;

            var fileName = Path.GetFileName(profileImageUrl);
            var filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "UploadedFiles",
                "ProfileImages",
                fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
