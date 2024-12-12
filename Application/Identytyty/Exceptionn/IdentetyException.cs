namespace Application.Identytyty.Exceptionn;

public abstract class IdentetyException: Exception
{
    public string? Login { get; }

    protected IdentetyException(string login, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        Login = login;
    }

    protected IdentetyException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
        
    }
}


public class LoginException() : IdentetyException($"Invalid login or password");

public class TicketUnknownException(string login,Exception innerException)
    : IdentetyException(login, $"Unknown exception for the ticket under login: {login}", innerException);