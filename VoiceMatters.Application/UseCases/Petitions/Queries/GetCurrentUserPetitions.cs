using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Helpers;

namespace VoiceMatters.Application.UseCases.Petitions.Queries
{
    public sealed record GetCurrentUserPetitions(
        List<Guid>? TagIds,
        int PageNumber = 1,
        int PageSize = 20,
        string SearchPhrase = "",
        SortType IncludeCompleted = SortType.Disable,
        SortType SortBySignQuantityPerDay = SortType.Descending,
        SortType SortBySignQuantity = SortType.Disable,
        SortType SortByDate = SortType.Disable
    ) : IRequest<List<PetitionDto>?>;
}
