using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Shared.DTOs
{
    public sealed record TagDto (
        Guid Id,
        string Name
    );
}