using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain;
using Domain.Users;
using MediatR;
namespace Application.Users.Commands;

public class UpdateUserCommand: IRequest<Result<User, UserException>>
{
    public required Guid UserId { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int PhoneNumber { get; init; }
}
public class UpdateUserCommandHandler(IUserRepository userRepository) : IRequestHandler<UpdateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var userId = new UserId(request.UserId);

        var existingUser = await userRepository.GetById(userId, cancellationToken);

        return await existingUser.Match(
            async u => await UpdateEntity(u, request.FirstName, request.LastName,request.PhoneNumber, cancellationToken),
            () => Task.FromResult<Result<User, UserException>>(new UserNotFoundException(userId)));
    }

    private async Task<Result<User, UserException>> UpdateEntity(
        User entity,
        string firstName,
        string lastName,
        int phoneNumber,
        CancellationToken cancellationToken)
    {
        try
        {
            entity.UpdateDetails(firstName, lastName,phoneNumber);

            return await userRepository.Update(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            return new UserUnknownException(entity.Id, exception);
        }
    }
}