using Domain.Category;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassLibrary1.Persistence.Configurations;

public class CategoryConfiguration: IEntityTypeConfiguration<Category> 
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new CategoryId(x));

        builder.Property(x => x.Size).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Name).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Material).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.InCountry).IsRequired().HasColumnType("varchar(255)");
    }
    
}