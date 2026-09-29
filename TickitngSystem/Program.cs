using ApplicationServices.Interfaces;
using ApplicationServices.Services;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Resend;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using TickitngSystem.Security;

const string CorsPolicy = "Frontend";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddHostedService<DeletedAccountCleanupService>();

var enforceHttps = builder.Configuration.GetValue("Https:Enforce", false);
var httpsPort = builder.Configuration.GetValue("Https:Port", 443);

builder.Services.AddHttpsRedirection(options =>
{
    options.HttpsPort = httpsPort;
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
});

builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
});

var jwtKey = JwtSigningKey.Decode(builder.Configuration["Jwt:Key"]);

var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "TaskFlow.Api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "TaskFlow.Frontend";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
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
            },
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Authentication is required or the access token is invalid."
                });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "You do not have permission to access this resource."
                });
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
    "https://taskflow-pal.xyz",
    "https://www.taskflow-pal.xyz"
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

builder.Services.AddScoped<
    IDashboardManager,
    DashboardManager>();


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

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var logger = context.RequestServices
            .GetRequiredService<ILoggerFactory>()
            .CreateLogger("GlobalExceptionHandler");

        logger.LogError(
            exception,
            "Unhandled exception for {Method} {Path}. TraceId: {TraceId}",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsJsonAsync(new
        {
            message = "An unexpected server error occurred.",
            traceId = context.TraceIdentifier
        });
    });
});

if (enforceHttps)
{
    if (!app.Environment.IsDevelopment())
        app.UseHsts();

    app.UseHttpsRedirection();
}

app.UseCors(CorsPolicy);
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    await next();
});
app.UseAuthentication();
app.UseRateLimiter();
app.UseAuthorization();

app.MapOpenApi(); // /openapi/v1.json
app.MapScalarApiReference(); // /scalar


app.MapGet("/", () => ErrorShared.System.ServerWorking).AllowAnonymous();

app.MapControllers();

app.Run();
