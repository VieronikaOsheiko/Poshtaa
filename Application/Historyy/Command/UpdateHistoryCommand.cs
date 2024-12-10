using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Historyy.Exceptionn;
using Domain.History;
using Domain.Parcels;
using Domain.Users;
using MediatR;
using Optional;

namespace Application.Historyy.Command;

public record UpdateHistoryCommand : IRequest<Result<History, HistoryException>> 
{
    public required Guid Id { get; init; }
    public required Guid UserId { get; init; }
    public required Guid ParcelId { get; init; }
    public required DateTime DataRecived { get; init; }
    
}

public class UpdateHistoryCommandHandler( IHistoryRepository _historyRepository) : IRequestHandler<UpdateHistoryCommand, Result<History, HistoryException>>
{
    public async Task<Result<History, HistoryException>> Handle(
        UpdateHistoryCommand request,
        CancellationToken cancellationToken)
    {
        var historyId = new HistoryId(request.Id);
        var history = await _historyRepository.GetById(historyId, cancellationToken);

        return await history.Match(
            async f =>
            {
                var existingCourseUser = await CheckDuplicated(historyId, cancellationToken);

                return await existingCourseUser.Match(
                    ef => Task.FromResult<Result<History, HistoryException>>(new HistoryAlreadyExistsException(ef.Id)),
                    async () => await UpdateEntity(f, new ParcelId(request.ParcelId), new UserId(request.UserId),request.DataRecived, cancellationToken));
            },
            () => Task.FromResult<Result<History, HistoryException>>(new HistoryNotFoundException(historyId)));
    }
    private async Task<Result<History, HistoryException>> UpdateEntity(
        History history,
        ParcelId parcelId,
        UserId userId,
        DateTime dataRecived,
        CancellationToken cancellationToken)
    {
        try
        {
            history.UpdateDetails(parcelId,userId, dataRecived);

            return await _historyRepository.Update(history, cancellationToken);
        }
        catch (Exception exception)
        {
            return new HistoryUnknownException(history.Id, exception);
        }
    }

    private async Task<Option<History>> CheckDuplicated(
        HistoryId historyId,
        CancellationToken cancellationToken)
    {
        var course = await _historyRepository.GetById(historyId, cancellationToken);

        return course.Match(
            f => f.Id == historyId ? Option.None<History>() : Option.Some(f), 
            Option.None<History>);
    }
}  