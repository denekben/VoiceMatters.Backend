using MediatR;

namespace VoiceMatters.Application.UseCases.Administration.Commands
{
    public sealed record UnblockPetition(Guid Id) : IRequest;

}
