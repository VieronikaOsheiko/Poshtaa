using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain.Parcels;
using MediatR;

namespace Application.Parcelss.Command;

public record UpdateParcelCommand : IRequest<Result<Parcel, ParcelException>>
{
    public required Guid ParcelId { get; init; }
    public required Guid UserId { get; init; }
    public required Guid ReceiverId { get; init; }
    public required Guid CategoryId { get; init; }
    public required int TrackNumber { get; init; }
    public required DateTime DateOfShipment { get; init; }
    public required string AddresToCome { get; init; }
    public required string Weight { get; init; }
}

public class UpdateParcelCommandHandler(IParcelRepository iparcelrepository)
    : IRequestHandler<UpdateParcelCommand, Result<Parcel, ParcelException>>
{
    public async Task<Result<Parcel, ParcelException>> Handle(UpdateParcelCommand request,
        CancellationToken cancellationToken)
    {
        var parcelId = new ParcelId(request.ParcelId);
        var existingParcel = await iparcelrepository.GetById(parcelId, cancellationToken);
        return await existingParcel.Match(
            async p => await UpdateEntity(p, request.Weight, request.TrackNumber, request.DateOfShipment,
                request.AddresToCome, cancellationToken),
            () => Task.FromResult<Result<Parcel, ParcelException>>(new ParcelNotFoundException(parcelId)));
    }

    private async Task<Result<Parcel, ParcelException>> UpdateEntity(
        Parcel parcel,
        string weight,
        int trackNumber,
        DateTime dateOfShipment,
        string addresToCome,

        CancellationToken cancellationToken)
    {
        try
        {
            parcel.UpdateDetails( trackNumber, weight, addresToCome);

            return await iparcelrepository.Update(parcel, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ParcelUnknownException(parcel.Id, exception);
        }
    }
}