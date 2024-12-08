using System.Reflection;
using Domain;
using Domain.Category;
using Domain.History;
using Domain.Parcels;
using Microsoft.EntityFrameworkCore;

namespace ClassLibrary1.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<History> Histories { get; set; }
    
    public DbSet<Parcel> Parcels { get; set; }
    public DbSet<Category> Categories { get; set; }
    

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(builder);
    }
}