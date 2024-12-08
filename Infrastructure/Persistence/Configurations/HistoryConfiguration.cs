using ClassLibrary1.Persistence.Converters;
using Domain.History;
using Domain.Parcels;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ClassLibrary1.Persistence.Configurations;

public class HistoryConfiguration: IEntityTypeConfiguration<History> 
{
    public void Configure(EntityTypeBuilder<History> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, x => new HistoryId(x));

        builder.Property(x => x.DataReceived)
            .HasConversion(new DateTimeUtcConverter())
            .HasDefaultValueSql("timezone('utc', now())");
        builder.Property(x => x.ParcelId)
            .HasConversion(x => x.Value, x => new ParcelId(x))
            .IsRequired();
        builder.HasOne(x => x.Parcel)
            .WithMany()
            .HasForeignKey(x => x.ParcelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(x => x.UserId)
            .HasConversion(x => x.Value, x => new UserId(x))
            .IsRequired();
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        

        

      
    }
    
}