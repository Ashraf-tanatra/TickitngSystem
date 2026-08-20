using ApplicationServices.Interfaces;
using ApplicationServices.Services;
using Domain.EntityManager;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


// ==============================
// Controllers
// ==============================

builder.Services.AddControllers();


// ==============================
// Database
// ==============================

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
// Account / Authentication
// ==============================

builder.Services.AddScoped<
    IAuthManager,
    AuthManager>();

builder.Services.AddScoped<
    IAccountRepository,
    AccountRepository>();


// ==============================
// Email
// ==============================

builder.Services.AddScoped<
    IEmailService,
    EmailService>();


// ==============================
// OpenAPI / Scalar
// ==============================

builder.Services.AddOpenApi();


var app = builder.Build();


// ==============================
// OpenAPI / Scalar
// ==============================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


// ==============================
// Controllers
// ==============================

app.MapControllers();


// ==============================
// Test Endpoint
// ==============================

app.MapGet("/", () => "Server is working!");


app.Run();