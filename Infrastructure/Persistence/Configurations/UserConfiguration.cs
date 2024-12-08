using Domain;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace ClassLibrary1.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new UserId(x));

        builder.Property(x => x.FirstName).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.LastName).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.PhoneNumber).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Login).IsRequired().HasColumnType("varchar(255)");

        builder.Property(x => x.Password).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.IsAdmin)
            .IsRequired()
            .HasDefaultValue(false);

        
    }
}