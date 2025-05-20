namespace VoiceMatters.Shared.DTOs
{
    public sealed record ProfilePlateDto(
        Guid Id,
        string FirstName,
        string LastName,
        string? Sex,
        string? ImageUuid,
        bool IsBlocked,
        RoleDto? Role
    );
}