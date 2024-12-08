using Domain.History;
using Domain.Parcels;
using Domain.Users;
using Optional;
namespace Application.Common.Interfaces.Repositories;

public interface IHistoryRepository
{
    Task<Option<History>> GetById(HistoryId id, CancellationToken cancellationToken);
    Task <List<History>> GetListParcelId(ParcelId parcelId, CancellationToken cancellationToken); 
    Task<Option<History>> GetByParcelIdAndUserId(ParcelId parcelId, UserId userId, CancellationToken cancellationToken);
    Task<History> Add(History courseuser, CancellationToken cancellationToken);
    Task<History> Update(History courseuser, CancellationToken cancellationToken);
    Task<History> Delete(History courseuser, CancellationToken cancellationToken);
}