//using Domain.Interfaces;
//using Infrastructure;
//using Infrastructure.Database;
//using Infrastructure.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;

//DataSeeder.RecreateCleanDatabase();
//DataSeeder.PopulateDatabase();

//using (var context = new AppDbContext())
//{
//    var service = new ProjectRepository(context);
//    service.SetProjectAsActive(2);
//    var result = service.GetAllProjectWorkedByEmployee(2);

//    foreach (var project in result)
//    {
//        Console.WriteLine(project);
//    }
//}
