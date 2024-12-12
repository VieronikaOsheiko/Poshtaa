using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;
namespace Apii.Modules.Errors;

public static class CategoryErrorHandler
{
    public static ObjectResult ToObjectResult(this CategoryException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                CategoryNotFoundException or
                    CategoryAlreadyExistsException => StatusCodes.Status409Conflict,
                CategoryUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Parcel error handler does not implemented")
            }
        };
    }
}