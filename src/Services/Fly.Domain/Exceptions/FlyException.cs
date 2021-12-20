namespace Fly.Domain.Exceptions
{
    public class FlyDomainException : Exception
    {
        public FlyDomainException(string name, string message, Exception ex)
            : base($"Service of entity \"{name}\" is failed. {message}", ex)
        {
        }
    }
}
