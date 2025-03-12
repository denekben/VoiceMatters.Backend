using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.News.Queries
{
    public sealed record GetNews(
        int PageNumber = 1,
        int PageSize = 20,
        string SearchPhrase = "",
        string SortBySignQuantityPerDay ="Disable",
        string SortBySignQuantity = "Disable",
        string SortByCompleteDate = "Descending"
    ) : IRequest<List<NewsDto>?>;
}
