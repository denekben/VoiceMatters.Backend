using MediatR;

namespace VoiceMatters.Application.UseCases.Petitions.Commands
{
    public sealed record SignPetition(Guid Id) : IRequest;
}
