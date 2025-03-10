using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Shared.DTOs;

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