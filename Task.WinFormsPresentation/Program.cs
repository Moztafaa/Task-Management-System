
using System;
using Microsoft.Extensions.DependencyInjection;
using Task.Infrastructure;
using Task.Application;
namespace Task.WinFormsPresentation;

using System.Windows.Forms;
using Task.Application.ServiceImplementation;
using Task.Application.ServiceInterface;
using Task.Domain.RepositoryInterface;
using Task.Infrastructure.DatabaseContext;
using Task.Infrastructure.RepositoryImplementation;
using Task.WinFormsPresentation.Forms;

static class Program
{
    public static ServiceProvider ServiceProvider { get; private set; } = null!;

    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Setup DIC
        var services = new ServiceCollection();
        ConfigureServices(services);
        ServiceProvider = services.BuildServiceProvider();

        // Main application loop - keeps showing login until user exits
        bool shouldContinue = true;
        while (shouldContinue)
        {
            // Start with Login Form
            var loginForm = ServiceProvider.GetRequiredService<LoginForm>();
            var loginResult = loginForm.ShowDialog();

            if (loginResult == DialogResult.OK)
            {
                // Show Dashboard
                var dashboardForm = ServiceProvider.GetRequiredService<DashboardForm>();
                var dashboardResult = dashboardForm.ShowDialog();

                // If dashboard returns Retry, it means user logged out and wants to login again
                // If dashboard returns anything else (Cancel, etc.), exit the application
                shouldContinue = (dashboardResult == DialogResult.Retry);
            }
            else
            {
                // User cancelled login or closed login form - exit application
                shouldContinue = false;
            }
        }

        ServiceProvider.Dispose();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Database Context
        services.AddDbContext<AppDbContext>();

        // Repositories
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ICategoryService, CategoryService>();

        // Forms
        services.AddScoped<LoginForm>();
        services.AddScoped<RegisterForm>();
        services.AddScoped<DashboardForm>();
        services.AddScoped<MainForm>();
        services.AddScoped<Task.WinFormsPresentation.Forms.TestForm>();
        services.AddScoped<LauncherForm>();
        services.AddScoped<CategoryManagementForm>();
    }
}
