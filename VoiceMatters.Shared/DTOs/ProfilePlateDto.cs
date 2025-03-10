using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Shared.DTOs
{
    public sealed record ProfilePlateDto(
        Guid Id,
        string FirstName,
        string LastName,
        string? Sex,
        string? ImageUuid,
        bool IsBlocked
    );
}