using Application.Users.Exceptions;
using Microsoft.AspNetCore.Mvc;
namespace Apii.Modules.Errors;

public static class ParcelErrorHandler
{
    public static ObjectResult ToObjectResult(this ParcelException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                ParcelNotFoundException or
                    ParcelAlreadyExistsException => StatusCodes.Status409Conflict,
                ParcelUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("Parcel error handler does not implemented")
            }
        };
    }
}