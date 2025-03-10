using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Entities.Pivots;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Petitions.Commands
{
    public sealed record CreatePetition(
        string Title,
        string TextPayload,
        List<string> Tags,
        List<CreateImageDto> Images
        ) : IRequest<PetitionDto?>;
}
