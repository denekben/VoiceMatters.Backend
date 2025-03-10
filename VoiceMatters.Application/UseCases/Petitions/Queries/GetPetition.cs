using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Petitions.Queries
{
    public sealed record GetPetition(Guid Id) : IRequest<PetitionDto?>;
}
