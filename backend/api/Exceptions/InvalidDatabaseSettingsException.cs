namespace api.Exceptions;

// exception for database settings, the application will crash if these are incorrect.
public class InvalidDatabaseSettingsException : Exception
{
    public InvalidDatabaseSettingsException()
    {
    }

    public InvalidDatabaseSettingsException(string message)
        : base(message)
    {
    }

    public InvalidDatabaseSettingsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}