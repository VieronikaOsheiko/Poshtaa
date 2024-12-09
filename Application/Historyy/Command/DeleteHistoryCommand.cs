using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Historyy.Exceptionn;
using Domain.History;
using MediatR;
namespace Application.Historyy.Command;

public class DeleteHistoryCommand : IRequest<Result<History, HistoryException>>
{
    public required Guid HistoryId { get; set; }
}

public class DeleteHistoryCommandHandler(IHistoryRepository historyRepository)
    : IRequestHandler<DeleteHistoryCommand, Result<History, HistoryException>>
{
    public async Task<Result<History, HistoryException>> Handle(DeleteHistoryCommand request,
        CancellationToken cancellationToken)
    {
        var historyId = new HistoryId(request.HistoryId);
        var existingHistory = await historyRepository.GetById(historyId, cancellationToken);
        return await existingHistory.Match<Task<Result<History, HistoryException>>>(
            async u => await DeleteEntity(u, cancellationToken),
            () => Task.FromResult<Result<History, HistoryException>>(new HistoryNotFoundException(historyId)));
    }

    public async Task<Result<History, HistoryException>> DeleteEntity(History history,
        CancellationToken cancellationToken)
    {
        try
        {
            return await historyRepository.Delete(history, cancellationToken);
        }
        catch (Exception exception)
        {
            return new HistoryUnknownException(history.Id, exception);
        }
    }
}