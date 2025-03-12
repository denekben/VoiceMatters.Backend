using Microsoft.AspNetCore.Http;

namespace VoiceMatters.Shared.DTOs
{
    public sealed record UpdateImageDto(
        IFormFile File,
        string? Uuid,
        uint Order,
        string? Caption
    );
}
