using Domain.Category;
using Domain.Users;

namespace Application.Users.Exceptions;

public abstract class CategoryException(CategoryId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public CategoryId UserId { get; } = id;
}

public class CategoryNotFoundException(CategoryId id) : CategoryException(id, $"User under id: {id} not found");

public class CategoryAlreadyExistsException(CategoryId id) : CategoryException(id, $"User already exists: {id}");

public class CategoryUnknownException(CategoryId id, Exception innerException)
    : CategoryException(id, $"Unknown exception for the user under id: {id}", innerException);