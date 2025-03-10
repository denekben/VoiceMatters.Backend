using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Shared.Exceptions
{
    public abstract class VoiceMattersException : Exception
    {
        protected VoiceMattersException() : base()
        {
            Errors = new Dictionary<string, string[]>();
        }
        protected VoiceMattersException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }
        public IDictionary<string, string[]> Errors { get; protected set; }
    }
}
