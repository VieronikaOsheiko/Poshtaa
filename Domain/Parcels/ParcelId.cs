namespace Domain.Parcels;

public record ParcelId(Guid Value)
{
    public static ParcelId New() => new(Guid.NewGuid());
    public static ParcelId Empty() => new(Guid.Empty);
    public override string ToString() => Value.ToString();
}