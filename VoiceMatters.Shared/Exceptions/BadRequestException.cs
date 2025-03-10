using VoiceMatters.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Shared.Exceptions
{
    public class BadRequestException : VoiceMattersException
    {
        public BadRequestException() : base() { }

        public BadRequestException(string message) : base(message) { }

    }
}
