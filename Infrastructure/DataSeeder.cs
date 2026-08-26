//using Domain.Entities;
//using Domain.Enum;

namespace Infrastructure
{
    public static class DataSeeder
    {
        public static async Task RecreateCleanDatabase()
        {
            using var context = new AppDbContext();

            await context.Database.EnsureDeletedAsync();
            await context.Database.EnsureCreatedAsync();
        }

        public static async Task PopulateDatabase()
        {
            using (var context = new AppDbContext())
            {
                //Employees
                var employees = LoadEmployee();
                context.Employees.AddRange(employees);
                await context.SaveChangesAsync();
                //Accounts
                var accounts = loadAccounts();
                context.Accounts.AddRange(accounts);
                await context.SaveChangesAsync();
                //Projects
                var projects = LoadProject();
                context.Projects.AddRange(projects);
                await context.SaveChangesAsync();
                //Tickets
                var tickets = LoadTicket();
                context.Tickets.AddRange(tickets);
                await context.SaveChangesAsync();
                //Employees Work Projects
                var empWorkProj = LoadEmployeeProject();
                context.ProjectEmployees.AddRange(empWorkProj);
                await context.SaveChangesAsync();
                // Attachments
                var attachment = LoadAttachments();
                context.Attachments.AddRange(attachment);
                await context.SaveChangesAsync();
                // Ticket Histories
                var ticketHistory = LoadTicketHistories();
                context.TicketHistories.AddRange(ticketHistory);
                await context.SaveChangesAsync();
            }

        }

        public static List<Employee> LoadEmployee() => new()
        {
            new Employee { FName = "John", LName = "Smith", Gender = Gender.M, Phone = "555-0101", IsDeleted = false },
            new Employee { FName = "Emily", LName = "Johnson", Gender = Gender.F, Phone = "555-0102", IsDeleted = false },
            new Employee { FName = "Michael", LName = "Brown", Gender = Gender.M, Phone = "555-0103", IsDeleted = false },
            new Employee { FName = "Sarah", LName = "Davis", Gender = Gender.F, Phone = "555-0104", IsDeleted = false },
            new Employee { FName = "David", LName = "Wilson", Gender = Gender.M, Phone = "555-0105", IsDeleted = false },
            new Employee { FName = "Jessica", LName = "Taylor", Gender = Gender.F, Phone = "555-0106",
                IsDeleted = true, DeletedAt = new DateTime(2026, 8, 1) }
        };
        public static List<Account> loadAccounts() => new()
        {
            new Account { Email = "john.smith@company.com", PasswordHash = "HASH_John123", EmployeeId = 1 },
            new Account { Email = "emily.johnson@company.com", PasswordHash = "HASH_Emily123", EmployeeId = 2 },
            new Account { Email = "michael.brown@company.com", PasswordHash = "HASH_Michael123", EmployeeId = 3 },
            new Account { Email = "sarah.davis@company.com", PasswordHash = "HASH_Sarah123", EmployeeId = 4 },
            new Account { Email = "david.wilson@company.com", PasswordHash = "HASH_David123", EmployeeId = 5 },
            new Account { Email = "jessica.taylor@company.com", PasswordHash = "HASH_Jessica123", EmployeeId = 6 }
        };
        public static List<Ticket> LoadTicket() => new()
        {
             new Ticket
            {
                TicketTitle = "Unable to Login", DueTo = new DateOnly(2026, 8, 28), CreatedAt = new DateOnly(2026, 8, 20),
                TicketStatus = TicketStatus.Pending, Priority = TicketPriority.Low,
                Description = "Employee is unable to log into the system.", EmployeeId = 1, TicketCreatedById = 1, ProjectId = 1
            },
            new Ticket
            {
                TicketTitle = "Database Connection Error", DueTo = new DateOnly(2026, 8, 27), CreatedAt = new DateOnly(2026, 8, 21),
                TicketStatus = TicketStatus.Pending, Priority = TicketPriority.High,
                Description = "Application cannot connect to the database.", EmployeeId = 3, TicketCreatedById = 1, ProjectId = 6
            },
            new Ticket
            {
                TicketTitle = "Incorrect Employee Information", DueTo = new DateOnly(2026, 8, 30),
                CreatedAt = new DateOnly(2026, 8, 22), TicketStatus = TicketStatus.Pending, Priority = TicketPriority.Medium,
                Description = "Employee information is displayed incorrectly.", EmployeeId = 4, TicketCreatedById = 1, ProjectId = 1
            },
            new Ticket
            {
                TicketTitle = "Website Loading Slowly", DueTo = new DateOnly(2026, 9, 2), CreatedAt = new DateOnly(2026, 8, 22),
                TicketStatus = TicketStatus.Pending, Priority = TicketPriority.High,
                Description = "The company website takes too long to load.", EmployeeId = 5, TicketCreatedById = 4, ProjectId = 4
            },
            new Ticket
            {
                TicketTitle = "Password Reset Request", DueTo = new DateOnly(2026, 8, 29), CreatedAt = new DateOnly(2026, 8, 23),
                TicketStatus = TicketStatus.Pending, Priority = TicketPriority.Low,
                Description = "Employee requested a password reset.", EmployeeId = 1, TicketCreatedById = 1, ProjectId = 1
            },
            new Ticket
            {
                TicketTitle = "Mobile Application Crash", DueTo = new DateOnly(2026, 9, 5), CreatedAt = new DateOnly(2026, 8, 23),
                TicketStatus = TicketStatus.Pending, Priority = TicketPriority.High,
                Description = "Mobile application crashes when opening the profile page.", EmployeeId = 3, TicketCreatedById = 3, ProjectId = 3
            },
            new Ticket
            {
                TicketTitle = "Missing Report Data", DueTo = new DateOnly(2026, 9, 3), CreatedAt = new DateOnly(2026, 8, 24),
                TicketStatus = TicketStatus.Pending, Priority = TicketPriority.Medium,
                Description = "Some records are missing from the reporting dashboard.", EmployeeId = 4, TicketCreatedById = 3, ProjectId = 7
            },
            new Ticket
            {
                TicketTitle = "API Authentication Issue", DueTo = new DateOnly(2026, 8, 31), CreatedAt = new DateOnly(2026, 8, 18),
                TicketStatus = TicketStatus.InProgress, Priority = TicketPriority.High,
                Description = "API authentication fails for some users.", EmployeeId = 5, TicketCreatedById = 5, ProjectId = 5
            },
            new Ticket
            {
                TicketTitle = "UI Alignment Problem", DueTo = new DateOnly(2026, 8, 15), CreatedAt = new DateOnly(2026, 8, 10),
                TicketStatus = TicketStatus.Completed, Priority = TicketPriority.Low,
                Description = "Several UI elements were not aligned correctly.", EmployeeId = 2, TicketCreatedById = 4, ProjectId = 4
            },
            new Ticket
            {
                TicketTitle = "Old Feature Request", DueTo = new DateOnly(2026, 8, 12), CreatedAt = new DateOnly(2026, 8, 5),
                TicketStatus = TicketStatus.Cancelled, Priority = TicketPriority.Medium,
                Description = "Feature request was cancelled by management.", EmployeeId = 5, TicketCreatedById = 4, ProjectId = 8
            },
            new Ticket
            {
                TicketTitle = "Previously Fixed Login Issue", DueTo = new DateOnly(2026, 8, 26), CreatedAt = new DateOnly(2026, 8, 16),
                TicketStatus = TicketStatus.Reopened, Priority = TicketPriority.High,
                Description = "A previously completed login issue has appeared again.", EmployeeId = 1, TicketCreatedById = 1, ProjectId = 1
            },
            new Ticket
            {
                TicketTitle = "Email Notification Problem", DueTo = new DateOnly(2026, 8, 10), CreatedAt = new DateOnly(2026, 8, 1),
                TicketStatus = TicketStatus.Done, Priority = TicketPriority.Low,
                Description = "Email notifications were not being sent correctly.", EmployeeId = 4, TicketCreatedById = 2, ProjectId = 2
            }
        };
        public static List<Project> LoadProject() => new()
        {
            new Project
            {
                ProjectName = "Employee Management System", ProjectDescription = "Internal system for managing employees and accounts.",
                ProjectStatus = ProjectStatus.Active, StartedAt = new DateOnly(2026, 1, 10),
                EndAt = new DateOnly(2026, 6, 30), ProjectManagerId = 1
            },
            new Project
            {
                ProjectName = "Customer Support Portal", ProjectDescription = "Portal for handling customer support tickets.",
                ProjectStatus = ProjectStatus.Active, StartedAt = new DateOnly(2026, 2, 1),
                EndAt = new DateOnly(2026, 8, 31), ProjectManagerId = 2
            },
            new Project
            {
                ProjectName = "Mobile Application", ProjectDescription = "Development of the company's mobile application.",
                ProjectStatus = ProjectStatus.Active, StartedAt = new DateOnly(2026, 3, 15),
                EndAt = new DateOnly(2026, 10, 15), ProjectManagerId = 3
            },
            new Project
            {
                ProjectName = "Website Redesign", ProjectDescription = "Redesign and modernization of the company website.",
                ProjectStatus = ProjectStatus.Completed, StartedAt = new DateOnly(2025, 9, 1),
                EndAt = new DateOnly(2026, 2, 28), ProjectManagerId = 4
            },
            new Project
            {
                ProjectName = "Security Improvement", ProjectDescription = "Improving application security and access control.",
                ProjectStatus = ProjectStatus.Active, StartedAt = new DateOnly(2026, 4, 1),
                EndAt = new DateOnly(2026, 12, 31), ProjectManagerId = 5
            },
            new Project
            {
                ProjectName = "Database Migration", ProjectDescription = "Migration of the existing database to the new platform.",
                ProjectStatus = ProjectStatus.OnHold,StartedAt = new DateOnly(2026, 5, 1),
                EndAt = new DateOnly(2026, 9, 30), ProjectManagerId = 1
            },
            new Project
            {
                ProjectName = "Reporting Dashboard", ProjectDescription = "Dashboard for employee and project reporting.",
                ProjectStatus = ProjectStatus.Active, StartedAt = new DateOnly(2026, 6, 1),
                EndAt = new DateOnly(2026, 11, 30), ProjectManagerId = 3
            },
            new Project
            {
                ProjectName = "Legacy System Replacement", ProjectDescription = "Replacing the old internal management system.",
                ProjectStatus = ProjectStatus.Cancelled, StartedAt = new DateOnly(2026, 1, 20),
                EndAt = new DateOnly(2026, 5, 20), ProjectManagerId = 4
            }
     };
        public static List<ProjectEmployee> LoadEmployeeProject() => new()
        {
            new ProjectEmployee { ProjectId = 1, EmployeeId = 2, Role = "Developer" },
            new ProjectEmployee { ProjectId = 1, EmployeeId = 3, Role = "Backend Developer" },
            new ProjectEmployee { ProjectId = 1, EmployeeId = 4, Role = "Tester" },

            new ProjectEmployee { ProjectId = 2, EmployeeId = 1, Role = "Support Specialist" },
            new ProjectEmployee { ProjectId = 2, EmployeeId = 5, Role = "Developer" },
            new ProjectEmployee { ProjectId = 2, EmployeeId = 6, Role = "Tester" },

            new ProjectEmployee { ProjectId = 3, EmployeeId = 1, Role = "Developer" },
            new ProjectEmployee { ProjectId = 3, EmployeeId = 4, Role = "UI Designer" },
            new ProjectEmployee { ProjectId = 3, EmployeeId = 5, Role = "Tester" },

            new ProjectEmployee { ProjectId = 4, EmployeeId = 2, Role = "Developer" },
            new ProjectEmployee { ProjectId = 4, EmployeeId = 3, Role = "UI Designer" },

            new ProjectEmployee { ProjectId = 5, EmployeeId = 1, Role = "Security Developer" },
            new ProjectEmployee { ProjectId = 5, EmployeeId = 4, Role = "Security Tester" },

            new ProjectEmployee { ProjectId = 6, EmployeeId = 3, Role = "Database Developer" },
            new ProjectEmployee { ProjectId = 6, EmployeeId = 5, Role = "Database Administrator" },

            new ProjectEmployee { ProjectId = 7, EmployeeId = 2, Role = "Developer" },
            new ProjectEmployee { ProjectId = 7, EmployeeId = 4, Role = "Data Analyst" },

            new ProjectEmployee { ProjectId = 8, EmployeeId = 1, Role = "Developer" },
            new ProjectEmployee { ProjectId = 8, EmployeeId = 5, Role = "System Analyst" }
        };

        public static List<TicketAttachments> LoadAttachments() => new();
        public static List<TicketHistory> LoadTicketHistories() => new();

    }
}