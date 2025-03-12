using Microsoft.AspNetCore.Http;

namespace VoiceMatters.Shared.DTOs
{
    public sealed record CreateImageDto(
        IFormFile File,
        string? Caption,
        uint Order
    );
}
