using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Task.Infrastructure.DatabaseContext;

// This factory is used by EF Core tools at design-time to create the DbContext
public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseSqlServer(AppDbContext.ConnectionString);
        return new AppDbContext(optionsBuilder.Options);
    }
}
