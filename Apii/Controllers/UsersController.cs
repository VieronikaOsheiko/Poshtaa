using Apii.Dtos;
using Apii.Modules.Errors;
using Application.Common.Interfaces.Queries;
using Application.Users.Commands;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Apii.Controllers
{
    [Route("users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ISender sender;
        private readonly IUserQueries userQueries;

        public UsersController(ISender sender, IUserQueries userQueries)
        {
            this.sender = sender;
            this.userQueries = userQueries;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken cancellationToken)
        {
           
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "userid");
            if (userIdClaim == null)
            {
                return Unauthorized();  
            }

            var userIdFromToken = new Guid(userIdClaim.Value);
            var entity = await userQueries.GetById(new UserId(userIdFromToken), cancellationToken);

            return entity.Match<ActionResult<UserDto>>(
                u => UserDto.FromDomainModel(u),
                () => NotFound());
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll(CancellationToken cancellationToken)
        {
            var entities = await userQueries.GetAll(cancellationToken);
            return entities.Select(UserDto.FromDomainModel).ToList();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] UserDto request, CancellationToken cancellationToken)
        {
            var input = new CreateUserCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Login = request.Login,
                Password = request.Password
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserDto>>(
                u => UserDto.FromDomainModel(u),
                e => e.ToObjectResult());
        }

        [Authorize]
        [HttpPut("{userId:guid}")]
        public async Task<ActionResult<UserDto>> Update([FromRoute] Guid userId, [FromBody] UserDto request, CancellationToken cancellationToken)
        {
            
            var input = new UpdateUserCommand
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserDto>>(
                user => UserDto.FromDomainModel(user),
                e => e.ToObjectResult());
        }

        [Authorize]
        [HttpDelete("{userId:guid}")]
        public async Task<ActionResult<UserDto>> Delete([FromRoute] Guid userId, CancellationToken cancellationToken)
        {
            var input = new DeleteUserCommand
            {
                UserId = userId
            };

            var result = await sender.Send(input, cancellationToken);

            return result.Match<ActionResult<UserDto>>(
                u => UserDto.FromDomainModel(u),
                e => e.ToObjectResult());
        }
    }
}
