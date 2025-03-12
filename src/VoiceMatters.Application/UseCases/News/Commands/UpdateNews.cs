using MediatR;
using VoiceMatters.Shared.DTOs;

namespace VoiceMatters.Application.UseCases.News.Commands
{
    public sealed record UpdateNews(Guid Id, string Title) : IRequest<NewsDto?>;
}
