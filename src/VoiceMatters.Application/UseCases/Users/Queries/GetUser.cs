using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Users.Queries
{
    public sealed record GetUser(Guid Id, bool AllowBlocked = false) : IRequest<ProfileDto>;
}
