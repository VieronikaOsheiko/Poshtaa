using Application.Historyy.Exceptionn;
using Microsoft.AspNetCore.Mvc;

namespace Apii.Modules.Errors;

public static class HistoryErrorHandler
{
    public static ObjectResult ToObjectResult(this HistoryException exception)
    {
        return exception switch
        {
            HistoryAlreadyExistsException => new ConflictObjectResult(new { error = "History already exists." }),
            ParceclNooFoundedException => new NotFoundObjectResult(new { error = "Parcel not found." }),
            InvalidHistoryDateException => new BadRequestObjectResult(new { error = "Invalid history date." }),
            HistoryUnknownException => new ObjectResult(new { error = "Unknown error occurred." }) { StatusCode = 500 },
            _ => new ObjectResult(new { error = "Unhandled error occurred." }) { StatusCode = 500 }
        };
    }
}