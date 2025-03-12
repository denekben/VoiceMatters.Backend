namespace VoiceMatters.Shared.DTOs
{
    public sealed record ProfileDto
    (
        Guid Id,
        string FirstName,
        string LastName,
        string? Phone,
        string Email,
        DateTime? BirthDate,
        string? Sex,
        string? ImageUuid,
        bool IsBlocked,
        int PetitionsCreated,
        int PetitionsSigned,
        DateTime CreatedDate
    );
}