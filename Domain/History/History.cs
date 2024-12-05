using Domain.Parcels;
using Domain.Users;

namespace Domain.History;

public class History
{
    public HistoryId Id { get; private set; }
    public ParcelId ParcelId { get;private set; }
    public Parcel? Parcel { get; private set;}
    public UserId UserId { get;private set; }
    public User? User { get; private set;} 
    public DateTime DataReceived { get;private set;  }

    public static History New(HistoryId id, ParcelId parcelId, UserId userId,  DateTime dataReceived)
    {
        return new History(id, parcelId, dataReceived, userId);
    }
    public History(HistoryId id, ParcelId parcelId, DateTime dataReceived, UserId userId)
    {
        Id = id;
        UserId = userId;
        ParcelId = parcelId;
        DataReceived = dataReceived;
    }
    public void UpdateDetails(ParcelId parcelId, UserId userId, DateTime dataReceived)
    {
        ParcelId = parcelId;
        UserId = userId;
        DataReceived = dataReceived;
    }
}