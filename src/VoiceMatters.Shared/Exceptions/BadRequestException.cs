namespace VoiceMatters.Shared.Exceptions
{
    public class BadRequestException : VoiceMattersException
    {
        public BadRequestException() : base() { }

        public BadRequestException(string message) : base(message) { }

    }
}
