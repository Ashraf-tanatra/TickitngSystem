//using Domain.Entities;
//using Domain.Enum;

//namespace Infrastructure.Database.Data
//{
//    public static class DataSeeder
//    {
//        public static void RecreateCleanDatabase()
//        {
//            using var context = new AppDbContext();

//            context.Database.EnsureDeleted();
//            context.Database.EnsureCreated();
//        }

//        public static void PopulateDatabase()
//        {
//            using (var context = new AppDbContext())
//            {
//                //Employees
//                var employees = LoadEmployee();
//                context.Employees.AddRange(employees);
//                context.SaveChanges();

//                //Accounts
//                var accounts = loadAccounts();
//                context.Accounts.AddRange(accounts);
//                context.SaveChanges();

//                //Projects
//                var projects = LoadProject();
//                context.Projects.AddRange(projects);
//                context.SaveChanges();

//                //Tickets
//                var tickets = LoadTicket();
//                context.Tickets.AddRange(tickets);
//                context.SaveChanges();


//                //Employees Work Projects
//                var empWorkProj = LoadEmployeeProject();
//                context.ProjectEmployees.AddRange(empWorkProj);
//                context.SaveChanges();

//            }

//        }
//        public static List<Employee> LoadEmployee() => new()
//        {
//        new Employee { FName = "John", LName = "Smith", Gender = 'M', Phone = "555-0101", IsDeleted = false },
//        new Employee { FName = "Sarah",LName = "Johnson",Gender = 'F',Phone = "555-0102",IsDeleted = false},
//        new Employee { FName = "Michael",LName = "Brown",Gender = 'M',Phone = "555-0103",IsDeleted = false}
//        };

//        public static List<Account> loadAccounts() => new()
//            {
//            new Account{Email = "john.smith@example.com",PasswordHash = "hashed_password_123",EmployeeId = 1},
//            new Account{Email = "sarah.johnson@example.com",PasswordHash = "hashed_password_456",EmployeeId = 2},
//            new Account{Email = "michael.brown@example.com",PasswordHash = "hashed_password_789",EmployeeId = 3}
//    };

//        public static List<Ticket> LoadTicket() => new()
//        {
//            new Ticket
//            {
//                TicketTitle = "Fix login issue",DueTo = DateTime.Now.AddDays(3),CreatedTime = DateTime.Now.AddDays(-2),
//                TicketStatus = TicketStatus.Pending,Priority = TicketPriority.Low,Description = "Users are unable to log in.",
//                EmployeeId = 1,ProjectId = 2, TicketCreatedById = 1
//            },
//            new Ticket
//                {
//                TicketTitle = "Update homepage",DueTo = DateTime.Now.AddDays(7),CreatedTime = DateTime.Now.AddDays(-1),
//                TicketStatus = TicketStatus.InProgress,Priority = TicketPriority.Medium,
//                Description = "Update the homepage layout and content.",EmployeeId = 2,ProjectId = 1, TicketCreatedById = 2
//                },
//            new Ticket
//                {
//                TicketTitle = "API authentication",DueTo = DateTime.Now.AddDays(5),CreatedTime = DateTime.Now,TicketStatus = TicketStatus.Done,
//                Priority = TicketPriority.High,Description = "Implement authentication for the API.",EmployeeId = 3,ProjectId = 3, TicketCreatedById = 1
//                }
//            };
//        public static List<Project> LoadProject() => new()
//        {
//            new Project{ProjectName = "Website Redesign",ProjectDescription = "Redesign the company website.",ProjectManagerId = 1    },
//            new Project{ProjectName = "Mobile Application",ProjectDescription = "Develop a new mobile application.",ProjectManagerId = 2},
//            new Project{ProjectName = "CRM System",ProjectDescription = "Build an internal customer management system.",ProjectManagerId = 1}

//            };
//        public static List<ProjectEmployee> LoadEmployeeProject() => new()
//        {
//            new ProjectEmployee {EmployeeId = 1,ProjectId = 1},
//            new ProjectEmployee {EmployeeId = 1,ProjectId = 3},
//            new ProjectEmployee {EmployeeId = 2,ProjectId = 1},
//            new ProjectEmployee {EmployeeId = 2,ProjectId = 2},
//            new ProjectEmployee {EmployeeId = 3,ProjectId = 2},
//            new ProjectEmployee {EmployeeId = 3,ProjectId = 3}
//        };

//    }
//}