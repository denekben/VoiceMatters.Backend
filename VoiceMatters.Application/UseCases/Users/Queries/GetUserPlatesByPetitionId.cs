using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Users.Queries
{
    public sealed record GetUserPlatesByPetitionId(
        Guid PetitionId,
        int PageNumber = 1,
        int PageSize = 10
    ) : IRequest<List<ProfilePlateDto>?>;
}
