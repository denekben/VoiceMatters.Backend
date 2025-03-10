using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        ProfilePlateDto? Creator,
        DateTime CreatedDate,
        DateTime? UpdatedDate
    );
}