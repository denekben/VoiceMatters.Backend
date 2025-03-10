using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Shared.DTOs
{
    public sealed record UpdateImageDto(
        IFormFile File,
        string? Uuid,
        uint Order,
        string? Caption
    );
}
