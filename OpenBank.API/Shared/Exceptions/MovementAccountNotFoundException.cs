public class MovementAccountNotFoundException : Exception
{
    public MovementAccountNotFoundException()
    {
    }

    public MovementAccountNotFoundException(string message) : base(message)
    {
    }

    public MovementAccountNotFoundException(string message, Exception inner) : base(message, inner)
    {
    }
}