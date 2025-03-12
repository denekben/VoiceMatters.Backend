using MediatR;

namespace VoiceMatters.Application.UseCases.Petitions.Commands
{
    public sealed record CompletePetition(Guid Id) : IRequest;
}
