using Domain.Parcels;
using Domain.Users;

namespace Application.Users.Exceptions;

public abstract class ParcelException(ParcelId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public ParcelId UserId { get; } = id;
}

public class ParcelNotFoundException(ParcelId id) : ParcelException(id, $"User under id: {id} not found");

public class ParcelAlreadyExistsException(ParcelId id) : ParcelException(id, $"User already exists: {id}");

public class UserParcelNotFoundException(UserId userId) : ParcelException(ParcelId.Empty(), $"Faculty under id: {userId} not found");

public class ParcelUnknownException(ParcelId id, Exception innerException)
    : ParcelException(id, $"Unknown exception for the user under id: {id}", innerException);