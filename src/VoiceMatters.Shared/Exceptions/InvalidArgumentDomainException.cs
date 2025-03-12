namespace VoiceMatters.Shared.Exceptions
{
    public class InvalidArgumentDomainException : VoiceMattersException
    {
        public InvalidArgumentDomainException() : base() { }
        public InvalidArgumentDomainException(string message) : base(message) { }
    }
}
