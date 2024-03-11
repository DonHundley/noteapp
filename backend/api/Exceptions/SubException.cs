namespace api.Exceptions;

// An exception for subscriptions
public class SubException : Exception
{
    public SubException()
    {
    }

    public SubException(string message)
        : base(message)
    {
    }

    public SubException(string message, Exception inner)
        : base(message, inner)
    {
    }
}