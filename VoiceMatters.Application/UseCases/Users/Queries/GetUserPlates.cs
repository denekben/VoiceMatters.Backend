using VoiceMatters.Shared.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoiceMatters.Application.UseCases.Users.Queries
{
    public sealed record GetUserPlates(
        int PageNumber = 1,
        int PageSize = 10,
        string SearchPhrase = ""
        ) : IRequest<List<ProfilePlateDto>?>;
}
