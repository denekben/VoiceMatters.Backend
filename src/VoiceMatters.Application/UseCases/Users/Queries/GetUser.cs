using VoiceMatters.Shared.DTOs;
using MediatR;

namespace VoiceMatters.Application.UseCases.Users.Queries
{
    public sealed record GetUser(Guid Id) : IRequest<ProfileDto>;
}
