using VoiceMatters.Shared.DTOs;
using MediatR;

namespace VoiceMatters.Application.UseCases.Users.Queries
{
    public sealed record GetUserPlates(
        int PageNumber = 1,
        int PageSize = 10,
        string SearchPhrase = ""
        ) : IRequest<List<ProfilePlateDto>?>;
}
