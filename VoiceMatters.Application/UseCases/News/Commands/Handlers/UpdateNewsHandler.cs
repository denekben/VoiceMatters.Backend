using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Domain.Entities;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;
using DomainNews = VoiceMatters.Domain.Entities.News;

namespace VoiceMatters.Application.UseCases.News.Commands.Handlers
{
    internal sealed class UpdateNewsHandler : IRequestHandler<UpdateNews, NewsDto?>
    {
        private readonly ILogger<UpdateNewsHandler> _logger;
        private readonly IHttpContextService _contextService;
        private readonly IPetitionRepository _petitionRepository;
        private readonly INewsRepository _newsRepository;

        public UpdateNewsHandler(ILogger<UpdateNewsHandler> logger,
            IHttpContextService contextService, IPetitionRepository petitionRepository,
            INewsRepository newsRepository)
        {
            _logger = logger;
            _contextService = contextService;
            _petitionRepository = petitionRepository;
            _newsRepository = newsRepository;
        }

        public async Task<NewsDto?> Handle(UpdateNews command, CancellationToken cancellationToken)
        {
            var creatorId = _contextService.GetCurrentUserId();

            var news = await _newsRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find news {command.Id}");

            var petition = await _petitionRepository.GetAsync(news.PetitionId, PetitionIncludes.Images | PetitionIncludes.Tags)
                ?? throw new BadRequestException($"Cannot find petition {news.PetitionId}");

            if (petition.CreatorId != creatorId)
                throw new BadRequestException($"Only petition creator can update news for this petition");

            var updatedNews = DomainNews.Create(news.Id, news.Title, news.PetitionId)
                ?? throw new BadRequestException($"Cannot update news {command.Id}");

            await _newsRepository.UpdateAsync(updatedNews);
            _logger.LogInformation($"News {command.Id} updated");

            updatedNews.Petition = petition;

            return updatedNews.AsDto();
        }
    }
}
