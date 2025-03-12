using MediatR;

namespace VoiceMatters.Application.UseCases.News.Commands
{
    public sealed record DeleteNews(Guid Id) : IRequest;
}
