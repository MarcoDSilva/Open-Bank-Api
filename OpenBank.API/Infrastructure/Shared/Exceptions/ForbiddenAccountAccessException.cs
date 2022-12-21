public class ForbiddenAccountAccessException : Exception
{
    public ForbiddenAccountAccessException()
    {
    }

    public ForbiddenAccountAccessException(string message) : base(message)
    {
    }

    public ForbiddenAccountAccessException(string message, Exception inner) : base(message, inner)
    {
    }
}