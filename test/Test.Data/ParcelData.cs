using Domain.Category;
using Domain.Parcels;
using Domain.Users;

namespace Test.Data;

public static class ParcelData
{
    public static Parcel MainParcel(UserId userId,CategoryId categoryId)
        => Parcel.New(ParcelId.New(), 1234, DateTime.UtcNow,"serpnya","12", userId,userId,categoryId);
}