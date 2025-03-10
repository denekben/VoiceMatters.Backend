using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Tags.Queries
{
    public sealed record GetTags(
        string SearchPhrase = "",
        int PageNumber = 1,
        int PageSize = 20
    ) : IRequest<List<TagDto>?>;
}
