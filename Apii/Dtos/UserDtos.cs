using Domain;

namespace Apii.Dtos;

public record UserDto(

    Guid? Id,
    string FirstName,
    string LastName,
    int PhoneNumber,
    string Login,
    string Password)
{

    public static UserDto FromDomainModel(User user)
        => new(
            Id: user.Id.Value,
            FirstName: user.FirstName,
            LastName: user.LastName,
            PhoneNumber: user.PhoneNumber,
            Login: user.Login,
            Password: user.Password);

}