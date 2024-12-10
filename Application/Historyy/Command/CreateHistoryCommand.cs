using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Historyy.Exceptionn;
using Domain.History;
using Domain.Parcels;
using Domain.Users;
using MediatR;

namespace Application.Historyy.Command;

public record CreateHistoryCommand : IRequest<Result<History, HistoryException>>
{
    public Guid UserId { get; init; }
    public Guid ParcelId { get; init; }
    public required DateTime DataReceived { get; init; }
}

public class CreateHistoryCommandHandler : IRequestHandler<CreateHistoryCommand, Result<History, HistoryException>>
{
    private readonly IHistoryRepository _historyRepository;
    private readonly IParcelRepository _parcelRepository;

    public CreateHistoryCommandHandler(IHistoryRepository historyRepository, IParcelRepository parcelRepository)
    {
        _historyRepository = historyRepository;
        _parcelRepository = parcelRepository;
    }

    public async Task<Result<History, HistoryException>> Handle(
        CreateHistoryCommand request,
        CancellationToken cancellationToken)
    {
        if (request.DataReceived.Date < DateTime.UtcNow.Date)
        {
            return new InvalidHistoryDateException(new ParcelId(request.ParcelId));
        }

        var parcelId = new ParcelId(request.ParcelId);
        var userId = new UserId(request.UserId);
        var currentHistory = await _parcelRepository.GetById(parcelId, cancellationToken);
        var history = await _historyRepository.GetListParcelId(parcelId, cancellationToken);
        var existingHistory = await _historyRepository.GetByParcelIdAndUserId(parcelId, userId, cancellationToken);

        return await existingHistory.Match(
            f => Task.FromResult<Result<History, HistoryException>>(
                new HistoryAlreadyExistsException(f.Id)),
            async () =>
            {
                var currentParcel = await _parcelRepository.GetById(parcelId, cancellationToken);

                return await currentParcel.Match(
                    async c =>
                    {
                        return await CreateEntity(parcelId, userId, request.DataReceived, cancellationToken);
                    },
                    () => Task.FromResult<Result<History, HistoryException>>(
                        new InvalidHistoryDateException(parcelId))
                );
            }
        );
    }



        private async Task<Result<History, HistoryException>> CreateEntity(
            ParcelId parentId,
            UserId userId,
            DateTime dataReceived,
            CancellationToken cancellationToken
        )
        {
            try
            {
                var entity = History.New(HistoryId.New(), parentId, userId, dataReceived);
                return await _historyRepository.Add(entity, cancellationToken);
            }
            catch (Exception exception)
            {
                return new HistoryUnknownException(HistoryId.Empty(), exception);
            }
        }
    }

