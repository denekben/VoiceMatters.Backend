using MediatR;

namespace VoiceMatters.Application.UseCases.Identity.Commands
{
    public sealed record RefreshExpiredToken(string AccessToken, string RefreshToken) : IRequest<string?>;
}
