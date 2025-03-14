namespace VoiceMatters.Shared.DTOs
{
    public sealed record PetitionDto(
        Guid Id,
        string Title,
        string TextPayload,
        uint SignQuantity,
        uint SignQuantityPerDay,
        bool IsCompleted,
        bool IsBlocked,
        bool SignedByCurrentUser,
        List<TagDto>? Tags,
        List<ImageDto>? Images,
        string? NewsTitle,
        ProfilePlateDto? Creator,
        DateTime CreatedDate,
        DateTime? UpdatedDate
    );
}