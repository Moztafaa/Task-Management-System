
using System;
using Microsoft.Extensions.DependencyInjection;
using Task.Infrastructure;
using Task.Application;
namespace Task.WinFormsPresentation;


using System.Windows.Forms;
using Task.Application.ServiceImplementation;
using Task.Application.ServiveInterface;
using Task.Domain.RepositoryInterface;
using Task.Infrastructure.DatabaseContext;
using Task.Infrastructure.RepositoryImplementation;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Setup DIC
        var services = new ServiceCollection();
        ConfigureServices(services);
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        Application.Run(serviceProvider.GetRequiredService<MainForm>());
    }



    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITaskService, TaskService>();

        services.AddTransient<MainForm>();
    }
}
