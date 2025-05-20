using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Petitions.Commands
{
    public sealed record UpdatePetition(
        Guid Id,
        string Title,
        string TextPayload,
        List<string>? Tags,
        List<UpdateImageDto> Images
        ) : IRequest<PetitionDto?>;
}
