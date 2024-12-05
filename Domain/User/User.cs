using Domain.Users;

namespace Domain;

public class User
{
    public UserId Id { get; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }

    public int PhoneNumber { get; private set; }
    public string Login { get; private set; }
    public string Password { get; private set; }
    public bool IsAdmin { get; private set; }

    
    private User(UserId id, string firstName, string lastName, int phoneNumber,string login, string password)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        Login = login;
        Password = password;
        IsAdmin = login == "admin"; 
    }

    public static User New(UserId id, string firstName, string lastName, int phoneNumber,string login, string password)
        => new User(id, firstName, lastName, phoneNumber,login, password);
    public void UpdateDetails(string firstName, string lastName, int phoneNumber)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
    }
    public void ChangeLogin(string login)
    {
        Login = login;
        IsAdmin = login == "admin"; 
    }
    public void ChangePassword(string password)
    {
        Password = password;
    }
}