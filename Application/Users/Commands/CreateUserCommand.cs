using Application.Common;
using Application.Common.Interfaces.Repositories;
using Application.Users.Exceptions;
using Domain;
using Domain.Users;
using MediatR;

namespace Application.Users.Commands;

public record CreateUserCommand : IRequest<Result<User, UserException>>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int PhoneNumber { get; init; }
    public required string Login { get; init; }
    public required string Password { get; init; }
}

public class CreateUserCommandHandler(
    IUserRepository userRepository
)
    : IRequestHandler<CreateUserCommand, Result<User, UserException>>
{
    public async Task<Result<User, UserException>> Handle(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.SearchByName(request.FirstName, cancellationToken);

        return await existingUser.Match(
            f => Task.FromResult<Result<User, UserException>>(new UserAlreadyExistsException(f.Id)),
            async () => await CreateEntity(request.FirstName, request.LastName,request.PhoneNumber,request.Login,request.Password, cancellationToken));
    }
    

    private async Task<Result<User, UserException>> CreateEntity(
        string firstName,
        string lastName,
        int phoneNumber,
        string login,
        string password,
        CancellationToken cancellationToken)
    {
        var userId = UserId.New();
        try
        {
            var entity = User.New(userId, firstName, lastName, phoneNumber,login,password);

            return await userRepository.Add(entity, cancellationToken);
        }
        catch (Exception exception)
        {
            Console.WriteLine($"Exception in CreateEntity: {exception.Message}");
            return new UserUnknownException(userId, exception);
        }
    }

}