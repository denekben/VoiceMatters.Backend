using MediatR;

namespace VoiceMatters.Application.UseCases.Administration.Commands
{
    public sealed record BlockUser(Guid Id) : IRequest;
}
