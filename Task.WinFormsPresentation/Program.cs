
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

        // Start with Login Form
        var loginForm = ServiceProvider.GetRequiredService<LoginForm>();
        var loginResult = loginForm.ShowDialog();

        if (loginResult == DialogResult.OK)
        {
            // Show Dashboard
            var dashboardForm = ServiceProvider.GetRequiredService<DashboardForm>();
            Application.Run(dashboardForm);
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
        services.AddTransient<LoginForm>();
        services.AddTransient<RegisterForm>();
        services.AddTransient<DashboardForm>();
        services.AddTransient<MainForm>();
        services.AddTransient<Task.WinFormsPresentation.Forms.TestForm>();
        services.AddTransient<LauncherForm>();
        services.AddTransient<CategoryManagementForm>();
    }
}
