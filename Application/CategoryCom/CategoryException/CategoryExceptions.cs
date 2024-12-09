using Domain.Category;

namespace Application.CategoryCom.CategoryException;

public abstract class CategoryExceptions(CategoryId id, string message, Exception? innerException = null)
    : Exception(message, innerException)
{
    public CategoryId UserId { get; } = id;
}

public class CategoryNotFoundExceptions(CategoryId id) : CategoryExceptions(id, $"User under id: {id} not found");

public class CategoryAlreadyExistsExceptions(CategoryId id) : CategoryExceptions(id, $"User already exists: {id}");

public class CategoryUnknownExceptions(CategoryId id, Exception innerException)
    : CategoryExceptions(id, $"Unknown exception for the user under id: {id}", innerException);