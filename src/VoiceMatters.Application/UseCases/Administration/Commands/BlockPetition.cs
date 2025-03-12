using MediatR;

namespace VoiceMatters.Application.UseCases.Administration.Commands
{
    public sealed record BlockPetition(Guid Id) : IRequest;
}
