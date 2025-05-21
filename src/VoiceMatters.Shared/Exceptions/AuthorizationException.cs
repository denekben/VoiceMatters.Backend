namespace VoiceMatters.Shared.Exceptions
{
    public class AuthorizationException : VoiceMattersException
    {
        public AuthorizationException() : base() { }

        public AuthorizationException(string message) : base(message) { }

    }
}
