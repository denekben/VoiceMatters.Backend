using MediatR;

namespace VoiceMatters.Application.UseCases.Petitions.Commands
{
    public sealed record DeletePetition(Guid Id) : IRequest;
}
