using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.Petitions.Queries
{
    public sealed record GetPetitions(
        List<Guid>? TagIds,
        int PageNumber = 1,
        int PageSize = 20,
        string SearchPhrase = "",
        string IncludeCompleted = "",
        string SortBySignQuantityPerDay = "Descending",
        string SortBySignQuantity = "Disable",
        string SortByDate = "Disable",
        bool AllowBlocked = false
    ) : IRequest<List<PetitionDto>?>;
}
