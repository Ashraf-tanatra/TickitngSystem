using ApplicationServices.Interfaces;
using ApplicationServices.Services;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.RateLimiting;
using Resend;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using System.Text;
using TickitngSystem.Security;

const string CorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHostedService<DeletedAccountCleanupService>();

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("Jwt:Key must contain at least 32 bytes.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TaskFlow.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TaskFlow.Frontend";

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = "email"
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var accountIdValue = context.Principal?.FindFirst(JwtClaimNames.AccountId)?.Value;
                var versionValue = context.Principal?.FindFirst(JwtClaimNames.AccountVersion)?.Value;

                if (!Guid.TryParse(accountIdValue, out var accountId) ||
                    !long.TryParse(versionValue, out var tokenVersion))
                {
                    context.Fail("The access token is missing required claims.");
                    return;
                }

                var dbContext = context.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
                var account = await dbContext.Accounts
                    .AsNoTracking()
                    .Include(item => item.Employee)
                    .FirstOrDefaultAsync(item => item.Id == accountId, context.HttpContext.RequestAborted);

                var accountVersion = (account?.UpdatedAt ?? account?.CreatedAt)?.Ticks;
                if (account == null || account.IsDeleted || !account.IsEmailVerified ||
                    account.Employee == null || account.Employee.IsDeleted ||
                    accountVersion != tokenVersion)
                {
                    context.Fail("The access token is no longer valid.");
                }
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

builder.Services.AddRateLimiter(options =>
{
    var apiRequestsPerMinute = Math.Max(
        1,
        builder.Configuration.GetValue("RateLimit:ApiRequestsPerMinute", 120));
    var anonymousRequestsPerMinute = Math.Max(
        1,
        builder.Configuration.GetValue("RateLimit:AnonymousRequestsPerMinute", 30));
    var authRequestsPerMinute = Math.Max(
        1,
        builder.Configuration.GetValue("RateLimit:AuthRequestsPerMinute", 10));

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
    {
        var employeeId = httpContext.User.FindFirst(JwtClaimNames.EmployeeId)?.Value;
        var isAuthenticated = httpContext.User.Identity?.IsAuthenticated == true &&
            !string.IsNullOrWhiteSpace(employeeId);
        var partitionKey = isAuthenticated
            ? $"user:{employeeId}"
            : $"ip:{httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown"}";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = isAuthenticated
                    ? apiRequestsPerMinute
                    : anonymousRequestsPerMinute,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.AddPolicy("auth", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = authRequestsPerMinute,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.OnRejected = async (context, cancellationToken) =>
    {
        if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
        {
            context.HttpContext.Response.Headers.RetryAfter =
                Math.Ceiling(retryAfter.TotalSeconds).ToString();
        }

        await context.HttpContext.Response.WriteAsJsonAsync(
            new { message = "Too many requests. Please try again later." },
            cancellationToken);
    };
});

var defaultAllowedOrigins = new[]
{
    "http://localhost:5173",
    "http://13.140.154.75:8081",
    "http://taskflow-pal.xyz",
    "https://taskflow-pal.xyz",
    "http://www.taskflow-pal.xyz",
    "https://www.taskflow-pal.xyz",
    "http://taskflow-pal.xyz:8081"
};

var configuredAllowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
    ?? [];

var allowedOrigins = defaultAllowedOrigins
    .Concat(configuredAllowedOrigins)
    .Distinct(StringComparer.OrdinalIgnoreCase)
    .ToArray();

builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicy, policy =>
    {
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var connectionString =
    builder.Configuration.GetConnectionString("constr");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception(
        "Connection string 'constr' is NULL or EMPTY!");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));


// ==============================
// Employee
// ==============================

builder.Services.AddScoped<
    IEmployeeManager,
    EmployeeManager>();

builder.Services.AddScoped<
    IEmployeeRepository,
    EmployeeRepository>();


// ==============================
// Project
// ==============================

builder.Services.AddScoped<
    IProjectManager,
    ProjectManager>();

builder.Services.AddScoped<
    IProjectRepository,
    ProjectRepository>();

builder.Services.AddScoped<
    IProjectEmployeeRepository,
    ProjectEmployeeRepository>();


// ==============================
// Ticket
// ==============================

builder.Services.AddScoped<
    ITicketManager,
    TicketManager>();

builder.Services.AddScoped<
    ITicketRepository,
    TicketRepository>();


// ==============================
// Account / Authentication
// ==============================

builder.Services.AddScoped<
    IAuthManager,
    AuthManager>();

builder.Services.AddScoped<
    IAccountManager,
    AccountManager>();

builder.Services.AddScoped<
    IAccountRepository,
    AccountRepository>();

builder.Services.AddOptions();

builder.Services.AddHttpClient<ResendClient>();

builder.Services.Configure<ResendClientOptions>(options =>
{
    options.ApiToken =
        builder.Configuration["Resend:ApiKey"]!;
});

builder.Services.AddTransient<IResend, ResendClient>();

builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAccessControlService, AccessControlService>();
builder.Services.AddSingleton<IAuthTokenService, JwtTokenService>();

// ==============================
// OpenAPI / Scalar
// ==============================

builder.Services.AddOpenApi("v1");


var app = builder.Build();

app.UseCors(CorsPolicy);
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapOpenApi(); // /openapi/v1.json
app.MapScalarApiReference(); // /scalar


app.MapGet("/", () => ErrorShared.System.ServerWorking).AllowAnonymous();

app.MapControllers();

app.Run();
