using Domain.History;
using Domain.Parcels;
using Domain.Users;

namespace Test.Data;

public static  class HistoryData
{
    public static History MainHistory(ParcelId parcelId, UserId userId)
        => History.New(HistoryId.New(), parcelId,userId, DateTime.UtcNow.AddYears(1));
}