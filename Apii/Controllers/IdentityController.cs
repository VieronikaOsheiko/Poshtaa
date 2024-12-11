using Apii.Dtos;
using Apii.Modules.Errors;
using Application.Identytyty.Comands;
using Microsoft.AspNetCore.Mvc;
using MediatR;

namespace Api.Controllers;

[ApiController]
[Route("identity")]
public class IdentityController(ISender sender) : ControllerBase
{

    [HttpPost("token")]
    public async Task<ActionResult<string>> GenerateToken([FromBody] TokenGenerationRequest request, CancellationToken cancellationToken)
    {
        var input = new CreateIdentetyCommand
        {
            Login = request.Login,
            Password = request.Password
        };

        var result = await sender.Send(input, cancellationToken);

        return result.Match<ActionResult<string>>(
            jwt => Ok(jwt), 
            e => e.ToObjectResult());
    }
}