using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Shared.Exceptions
{
    public class InvalidArgumentDomainException : VoiceMattersException
    {
        public InvalidArgumentDomainException() : base() { }
        public InvalidArgumentDomainException(string message) : base(message) { }
    }
}
