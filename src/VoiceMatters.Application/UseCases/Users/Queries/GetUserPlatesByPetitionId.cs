using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Users.Queries
{
    public sealed record GetUserPlatesByPetitionId(
        Guid PetitionId,
        int PageNumber = 1,
        int PageSize = 10,
        bool AllowBlocked = false
    ) : IRequest<List<ProfilePlateDto>?>;
}
