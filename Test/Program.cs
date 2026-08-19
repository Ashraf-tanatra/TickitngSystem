
//using Domain.Interfaces;
////using Infrastructure;
////using Infrastructure.Database;
////using Infrastructure.Repositories;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.Extensions.Configuration;

using Infrastructure;


DataSeeder.RecreateCleanDatabase();
DataSeeder.PopulateDatabase();


//using Domain.Interfaces;
//using Infrastructure;
//using Infrastructure.Repositories;

//var configuration = new ConfigurationBuilder()
//    .SetBasePath(Directory.GetCurrentDirectory())
//    .AddJsonFile("appsettings.json")
//    .Build();

//var connectionString = configuration["constr"];

//var options = new DbContextOptionsBuilder<AppDbContext>()
//    .UseSqlServer(connectionString)
//    .Options;

//using var context = new AppDbContext();

//IEmployeeRepository repository = new EmployeeRepository(context);


//// Test GetAll
//Console.WriteLine("===== ALL EMPLOYEES =====");

//var employees = repository.GetAll();

//foreach (var employee in employees)
//{
//    Console.WriteLine(
//        $"{employee.Id} - {employee.FName} {employee.LName}"
//    );
//}
//using Microsoft.Extensions.Configuration;
//using Infrastructure.Database;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Infrastructure;


//internal class Program
//{
//    private static void Main(string[] args)
//    {
//        using var context = new AppDbContext();

//        Console.WriteLine("Connected to database!");


//        // ==============================
//        // Test Employees
//        // ==============================

//        Console.WriteLine("\n===== EMPLOYEES =====");

//        var employees = context.Employees.ToList();

//        foreach (var employee in employees)
//        {
//            Console.WriteLine(
//                $"ID: {employee.Id} | " +
//                $"Name: {employee.FName} {employee.LName} | " +
//                $"Phone: {employee.Phone}"
//            );
//        }


//        // ==============================
//        // Test Projects
//        // ==============================

//        Console.WriteLine("\n===== PROJECTS =====");

//        var projects = context.Projects
//            .Include(p => p.ProjectManager)
//            .ToList();

//        foreach (var project in projects)
//        {
//            Console.WriteLine(
//                $"Project ID: {project.Id} | " +
//                $"Name: {project.ProjectName} | " +
//                $"Manager: {project.ProjectManager?.FName}"
//            );
//        }


//        // ==============================
//        // Test Tickets
//        // ==============================

//        Console.WriteLine("\n===== TICKETS =====");

//        var tickets = context.Tickets
//            .Include(t => t.Employee)
//            .Include(t => t.Project)
//            .ToList();

//        foreach (var ticket in tickets)
//        {
//            Console.WriteLine(
//                $"Ticket ID: {ticket.TicketId} | " +
//                $"Title: {ticket.TicketTitle} | " +
//                $"Employee: {ticket.Employee?.FName} | " +
//                $"Project: {ticket.Project?.ProjectName}"
//            );
//        }


//        // ==============================
//        // Test Project Employees
//        // ==============================

//        Console.WriteLine("\n===== PROJECT EMPLOYEES =====");

//        var projectEmployees = context.ProjectEmployees
//            .Include(pe => pe.Project)
//            .Include(pe => pe.Employee)
//            .ToList();

//        foreach (var pe in projectEmployees)
//        {
//            Console.WriteLine(
//                $"Project: {pe.Project.ProjectName} | " +
//                $"Employee: {pe.Employee.FName} {pe.Employee.LName}"
//            );
//        }
//    }
//}