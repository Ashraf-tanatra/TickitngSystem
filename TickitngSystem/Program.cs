using ApplicationServices.Interfaces;
using ApplicationServices.Services;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Resend;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllers();



builder.Services.AddHostedService<DeletedAccountCleanupService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var connectionString =
    builder.Configuration.GetConnectionString("constr");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception(
        "Connection string 'constr' is NULL or EMPTY!");
}

Console.WriteLine(
    "CONNECTION STRING = " + connectionString);

builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlServer(connectionString));


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
// Auto BackUp
// ==============================

builder.Services.AddScoped<DeletedAccountCleanupService>();

// ==============================
// Project
// ==============================

builder.Services.AddScoped<
    IProjectManager,
    ProjectManager>();

builder.Services.AddScoped<
    IProjectRepository,
    ProjectRepository>();


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

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("constr")));

// ==============================
// OpenAPI / Scalar
// ==============================

builder.Services.AddOpenApi("v1");


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // /openapi/v1.json
    app.MapScalarApiReference(); // /scalar
}


app.MapGet("/", () => "Server is working!");

app.MapControllers();

app.Run();