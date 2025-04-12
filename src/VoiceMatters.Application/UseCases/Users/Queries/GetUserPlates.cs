using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Users.Queries
{
    public sealed record GetUserPlates(
        int PageNumber = 1,
        int PageSize = 10,
        string SearchPhrase = "",
        bool AllowBlocked = false
        ) : IRequest<List<ProfilePlateDto>?>;
}
