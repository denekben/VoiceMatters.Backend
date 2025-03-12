using VoiceMatters.Shared.DTOs;
using MediatR;

namespace VoiceMatters.Application.UseCases.Identity.Commands
{
    public sealed record SignIn(string Email, string Password) : IRequest<TokensDto?>;
}
