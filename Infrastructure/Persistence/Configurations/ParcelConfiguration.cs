using ClassLibrary1.Persistence.Converters;
using Domain.Category;
using Domain.Parcels;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassLibrary1.Persistence.Configurations;

public class ParcelConfiguration : IEntityTypeConfiguration<Parcel> 
{
    public void Configure(EntityTypeBuilder<Parcel> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new ParcelId(x));

        builder.Property(x => x.AddresToCome).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.Weight).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.TrackNumber).IsRequired().HasColumnType("varchar(255)");
        builder.Property(x => x.DateOfShipment)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();

        builder.Property(x => x.ReceiverId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.CategoryId)
            .HasConversion(x => x.Value, x => new CategoryId(x))
            .IsRequired();
    }
    
}