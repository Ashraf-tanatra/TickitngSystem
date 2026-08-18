using ApplicationServices.Interfaces;
using Domain.EntityManager;
using Domain.Interfaces;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("constr");

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new Exception("Connection string 'constr' is NULL or EMPTY!");
}

Console.WriteLine("CONNECTION STRING = " + connectionString);

builder.Services.AddControllers();
builder.Services.AddOpenApi("v1");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("constr")));

builder.Services.AddScoped<IEmployeeManager, EmployeeManager>();
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<IProjectManager, ProjectManager>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<ITicketManager, TicketManager>();
builder.Services.AddScoped<ITicketRepository, TicketRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Exposes the JSON endpoint (e.g., /openapi/v1.json)
    app.MapScalarApiReference();
}
app.MapGet("/", () => "Server is working!");
app.MapControllers();

app.Run();