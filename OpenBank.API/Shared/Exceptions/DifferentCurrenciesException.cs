public class DifferentCurrenciesException : Exception
{
    public DifferentCurrenciesException()
    {
    }

    public DifferentCurrenciesException(string message) : base(message)
    {
    }

    public DifferentCurrenciesException(string message, Exception inner) : base(message, inner)
    {
    }
}