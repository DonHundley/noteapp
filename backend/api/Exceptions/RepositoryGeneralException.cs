namespace api.Exceptions;

public class RepositoryGeneralException : Exception
{
    public RepositoryGeneralException()
    {
    }

    public RepositoryGeneralException(string message)
        : base(message)
    {
    }

    public RepositoryGeneralException(string message, Exception inner)
        : base(message, inner)
    {
    }
}