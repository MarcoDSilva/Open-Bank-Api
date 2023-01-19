public class IllegalTokenException : Exception
{
    public IllegalTokenException()
    {
    }

    public IllegalTokenException(string message) : base(message)
    {
    }

    public IllegalTokenException(string message, Exception inner) : base(message, inner)
    {
    }
}