public class LowerBalanceException : Exception
{
    public LowerBalanceException()
    {
    }

    public LowerBalanceException(string message) : base(message)
    {
    }

    public LowerBalanceException(string message, Exception inner) : base(message, inner)
    {
    }
}