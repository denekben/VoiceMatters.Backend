using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;
using DomainNews = VoiceMatters.Domain.Entities.News;

namespace VoiceMatters.Application.UseCases.News.Commands.Handlers
{
    public sealed class CreateNewsHandler : IRequestHandler<CreateNews, NewsDto?>
    {
        private readonly ILogger<CreateNewsHandler> _logger;
        private readonly IHttpContextService _contextService;
        private readonly IPetitionRepository _petitionRepository;
        private readonly INewsRepository _newsRepository;

        public CreateNewsHandler(ILogger<CreateNewsHandler> logger,
            IHttpContextService contextService, IPetitionRepository petitionRepository,
            INewsRepository newsRepository)
        {
            _logger = logger;
            _contextService = contextService;
            _petitionRepository = petitionRepository;
            _newsRepository = newsRepository;
        }

        public async Task<NewsDto?> Handle(CreateNews command, CancellationToken cancellationToken)
        {
            var creatorId = _contextService.GetCurrentUserId();

            var petition = await _petitionRepository.GetAsync(command.PetitionId, PetitionIncludes.Tags | PetitionIncludes.Images)
                ?? throw new BadRequestException($"Cannot get petition {command.PetitionId}");

            if (petition.CreatorId != creatorId)
                throw new BadRequestException("Authorization error");

            if (!petition.IsCompleted)
                throw new BadRequestException("Cannot create news for uncompleted petition");

            if (petition.IsBlocked)
                throw new BadRequestException("Cannot create news for blocked petition");

            var news = DomainNews.Create(command.Title, command.PetitionId)
                ?? throw new BadRequestException("Cannot create news");

            await _newsRepository.AddAsync(news);
            _logger.LogInformation($"News {news.Id} created");

            news.Petition = petition;

            return news.AsDto();
        }
    }
}
