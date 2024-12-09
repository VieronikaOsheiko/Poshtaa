using Domain.Parcels;
using Domain.Users;
using Optional;
namespace Application.Common.Interfaces.Repositories;

public interface IParcelRepository
{
    Task<Parcel> Add(Parcel user, CancellationToken cancellationToken);
    Task<Parcel> Update(Parcel user, CancellationToken cancellationToken);
    Task<Parcel> Delete(Parcel user, CancellationToken cancellationToken);
    Task<Option<Parcel>> GetById(ParcelId id,CancellationToken cancellationToken);
    Task<Option<Parcel>> GetByWeightAndAdrres(
        string weigt,
        string adrress,
        CancellationToken cancellationToken);
    
}