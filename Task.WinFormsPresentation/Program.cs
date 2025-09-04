
using System;
using Microsoft.Extensions.DependencyInjection;
using Task.Infrastructure;
using Task.Application;
namespace Task.WinFormsPresentation;


using System.Windows.Forms;
using Task.Infrastructure.DatabaseContext;

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
        Application.Run(serviceProvider.GetRequiredService<Form1>());
    }



    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<AppDbContext>();
        services.AddTransient<Form1>();
    }
}
