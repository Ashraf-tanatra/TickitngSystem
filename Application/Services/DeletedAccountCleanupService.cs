using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApplicationServices.Services
{
    public class DeletedAccountCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DeletedAccountCleanupService> _logger;
        private readonly string _profileImagesFolder;

        public DeletedAccountCleanupService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<DeletedAccountCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            var storageRoot = configuration["FileStorage:RootPath"];
            _profileImagesFolder = Path.Combine(
                string.IsNullOrWhiteSpace(storageRoot)
                    ? Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles")
                    : Path.GetFullPath(storageRoot),
                "ProfileImages");
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

                    var profileImageUrls = accounts
                        .Select(account => account.Employee.ProfileImageUrl)
                        .ToList();

                    foreach (var account in accounts)
                    {
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
                    }

                    if (accounts.Any())
                    {
                        await context.SaveChangesAsync(
                            stoppingToken);

                        foreach (var profileImageUrl in profileImageUrls)
                            DeleteProfileImage(profileImageUrl);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "{Message}",
                        ErrorShared.System.CleanupErrorPrefix);
                }

                await Task.Delay(
                    TimeSpan.FromHours(24),
                    stoppingToken);
            }
        }

        private void DeleteProfileImage(string? profileImageUrl)
        {
            if (string.IsNullOrWhiteSpace(profileImageUrl))
                return;

            var fileName = Path.GetFileName(profileImageUrl);
            var filePath = Path.Combine(_profileImagesFolder, fileName);

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}
