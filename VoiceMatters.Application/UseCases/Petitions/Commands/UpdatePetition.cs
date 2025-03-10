using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Petitions.Commands
{
    public sealed record UpdatePetition(
        Guid Id,
        string Title,
        string TextPayload,
        List<string> Tags,
        List<UpdateImageDto> Images
        ) : IRequest<PetitionDto?>;
}
