namespace VoiceMatters.Shared.DTOs
{
    public sealed record NewsDto(
        Guid Id,
        string Title,
        Guid PetitionId,
        List<ImageDto>? PetitionImages,
        List<TagDto>? PetitionTags,
        uint SignQuantity,
        bool IsPetitionBlocked
    );
}