using Application.Identytyty.Exceptionn;
using Microsoft.AspNetCore.Mvc;

namespace Apii.Modules.Errors;

public static  class IdentetyErrorHandler
{
    public static ObjectResult ToObjectResult(this IdentetyException exception)
    {
        return new ObjectResult(exception.Message)
        {
            StatusCode = exception switch
            {
                LoginException => StatusCodes.Status401Unauthorized,
                TicketUnknownException => StatusCodes.Status500InternalServerError,
                _ => throw new NotImplementedException("User error handler does not implemented")
            }
        };
    }
}