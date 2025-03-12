namespace VoiceMatters.Shared.DTOs
{
    public sealed record ImageDto(
        Guid Id,
        string Uuid,
        string Caption,
        uint Order
    );
}