using Domain.Parcels;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IParcelQueries
{
    Task<IReadOnlyList<Parcel>> GetAll(CancellationToken cancellationToken);
    Task<Option<Parcel>> GetById(ParcelId id, CancellationToken cancellationToken);
}