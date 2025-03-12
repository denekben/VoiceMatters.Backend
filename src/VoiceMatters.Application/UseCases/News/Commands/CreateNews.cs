using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.News.Commands
{
    public sealed record CreateNews(Guid PetitionId, string Title) : IRequest<NewsDto?>;
}
