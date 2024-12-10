using Domain.Parcels;

namespace Apii.Dtos;

public record ParcelDtos(
    Guid? Id,
    int TrackNumber,
    DateTime DateOfShipment,
    string AddresToCome,
    string Weight,
    Guid UserId,
    Guid ReceiverId,
    Guid CategoryId,
    UserDto? User
)
{
    public static ParcelDtos FromDomainModel(Parcel parcel)
        => new(
            Id: parcel.Id.Value,
            TrackNumber: parcel.TrackNumber,
            DateOfShipment: parcel.DateOfShipment,
            AddresToCome: parcel.AddresToCome,
            Weight: parcel.Weight,
            UserId: parcel.UserId.Value,
            ReceiverId: parcel.ReceiverId.Value,
            CategoryId: parcel.CategoryId.Value,
            User: parcel.User == null ? null : UserDto.FromDomainModel(parcel.User));
}