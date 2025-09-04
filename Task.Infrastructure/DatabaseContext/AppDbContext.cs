using System;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using Task.Domain.Entities;

namespace Task.Infrastructure.DatabaseContext;

public class AppDbContext : DbContext
{
    static AppDbContext()
    {
        try
        {
            string? dir = Directory.GetCurrentDirectory();
            while (!string.IsNullOrEmpty(dir))
            {
                var envPath = Path.Combine(dir, ".env");
                if (File.Exists(envPath))
                {
                    Env.Load(envPath);
                    break;
                }
                dir = Directory.GetParent(dir)?.FullName;
            }
        }
        catch
        {
            // Ignore errors related to loading .env file
        }
    }
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }


    public static string ConnectionString =>
        $"Server={Environment.GetEnvironmentVariable("DB_SERVER") ?? "localhost,1434"};" +
        $"Database={Environment.GetEnvironmentVariable("DB_NAME") ?? "TaskManagementSystemDB"};" +
        $"User Id=SA;" +
        $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};" +
        $"TrustServerCertificate=true;";

    public DbSet<TaskItem> Tasks { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
}
