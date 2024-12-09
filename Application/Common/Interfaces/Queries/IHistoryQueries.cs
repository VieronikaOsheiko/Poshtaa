using Domain.History;
using Optional;

namespace Application.Common.Interfaces.Queries;

public interface IHistoryQueries
{
    Task<IReadOnlyList<History>> GetAll(CancellationToken cancellationToken);
    Task<Option<History>> GetById(HistoryId id, CancellationToken cancellationToken);
}