using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Shared.DTOs
{
    public sealed record CreateImageDto(
        IFormFile File,
        string? Caption,
        uint Order
    );
}
