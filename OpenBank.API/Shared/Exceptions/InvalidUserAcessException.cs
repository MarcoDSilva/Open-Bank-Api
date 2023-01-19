public class InvalidUserAccessException : Exception
{
    public InvalidUserAccessException()
    {
    }

    public InvalidUserAccessException(string message) : base(message)
    {
    }

    public InvalidUserAccessException(string message, Exception inner) : base(message, inner)
    {
    }
}