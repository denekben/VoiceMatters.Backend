using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Petitions.Commands
{
    public sealed record CreatePetition(
        string Title,
        string TextPayload,
        List<string>? Tags,
        List<CreateImageDto> Images
        ) : IRequest<PetitionDto?>;
}
