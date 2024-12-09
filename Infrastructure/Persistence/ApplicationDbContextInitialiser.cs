using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1.Persistence;

public class ApplicationDbContextInitialiser(ApplicationDbContext context)
{
    public async Task InitializeAsync()
    {
        await context.Database.MigrateAsync();
    }
}