// using System;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using DotNetEnv;
// using Task.Infrastructure.DatabaseContext;

// namespace Task.Infrastructure;

// public static class DependencyInjection
// {
//     public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
//     {

//         var envPath = Path.Combine(Directory.GetCurrentDirectory(), ".env");
//         if (File.Exists(envPath))
//         {
//             Env.Load(envPath);
//         }

//         // Register other infrastructure services here if needed
//         services.AddDbContext<AppDbContext>(options =>
//         {
//             var connectionString = AppDbContext.ConnectionString;
//             options.UseSqlServer(connectionString);
//         });

//         return services;
//     }

// }
