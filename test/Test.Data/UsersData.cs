using Domain;
using Domain.Users;

namespace Test.Data;

public static class UsersData
{
    public static User MainUser()
        => User.New(UserId.New(), "User First Name 1", "UserLastName1", 1234567890, "user1", "password1");
    public static User SecondUser()
        => User.New(UserId.New(), "User2", "UserLastName2", 1234567890, "user2", "password2");
}