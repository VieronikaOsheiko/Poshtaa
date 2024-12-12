using Domain.History;

namespace Apii.Dtos;

public record HistoryDtos 
(
    Guid? Id,
    Guid UserId,
    Guid ParcelId,
    DateTime DataReceived
    
)
{
    public static HistoryDtos FromDomainModel(History history)
        => new(
            Id: history.Id.Value,
            UserId: history.UserId.Value,
            ParcelId: history.ParcelId.Value,
            DataReceived: history.DataReceived);
}