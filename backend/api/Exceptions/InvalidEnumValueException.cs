namespace api.Exceptions;

// exception for enums
public class InvalidEnumValueException : Exception
{
    public InvalidEnumValueException()
    {
    }

    public InvalidEnumValueException(string message)
        : base(message)
    {
    }

    public InvalidEnumValueException(string message, Exception inner)
        : base(message, inner)
    {
    }
}