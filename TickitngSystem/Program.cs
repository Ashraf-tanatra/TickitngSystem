using ApplicationServices.Interfaces;
using ApplicationServices.Services;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllers();



builder.Services.AddHostedService<DeletedAccountCleanupService>();


var connectionString =
    builder.Configuration.GetConnectionString("constr");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception(
        "Connection string 'constr' is NULL or EMPTY!");
}

Console.WriteLine(
    "CONNECTION STRING = " + connectionString);

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


// ==============================
// OpenAPI / Scalar
// ==============================

builder.Services.AddOpenApi("v1");


var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.MapGet("/", () => "Server is working!");

app.MapControllers();

app.Run();