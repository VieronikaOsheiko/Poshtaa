using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain;
using Domain.Parcels;
using MediatR;

namespace Application.Users.Commands;

public record DeleteParcelCommand : IRequest<Result<Parcel, ParcelException>>
{
    public required Guid ParcelId { get; init; }
    
}

public class DeleteParcelCommandHandler(IParcelRepository parcelRepository)
    : IRequestHandler<DeleteParcelCommand, Result<Parcel, ParcelException>>
{
    public async Task<Result<Parcel, ParcelException>> Handle(
        DeleteParcelCommand request,
        CancellationToken cancellationToken)
    {
        var parcelId = new ParcelId(request.ParcelId);

        var existingParcel = await parcelRepository.GetById(parcelId, cancellationToken);

        return await existingParcel.Match<Task<Result<Parcel, ParcelException>>>(
            async p => await DeleteEntity(p, cancellationToken),
            () => Task.FromResult<Result<Parcel, ParcelException>>(new ParcelNotFoundException(parcelId)));
    }

    private async Task<Result<Parcel, ParcelException>> DeleteEntity(Parcel parcel, CancellationToken cancellationToken)
    {
        try
        {
            return await parcelRepository.Delete(parcel, cancellationToken);
        }
        catch (Exception exception)
        {
            return new ParcelUnknownException(parcel.Id, exception);
        }
    }
}