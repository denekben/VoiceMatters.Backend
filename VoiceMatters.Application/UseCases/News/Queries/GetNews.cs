using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Helpers;

namespace VoiceMatters.Application.UseCases.News.Queries
{
    public sealed record GetNews(
        int PageNumber = 1,
        int PageSize = 20,
        string SearchPhrase = "",
        SortType SortBySignQuantityPerDay = SortType.Disable,
        SortType SortBySignQuantity = SortType.Disable,
        SortType SortByCompleteDate = SortType.Descending
    ) : IRequest<List<NewsDto>?>;
}
