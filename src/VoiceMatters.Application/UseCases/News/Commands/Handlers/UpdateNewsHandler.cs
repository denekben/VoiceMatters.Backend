using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Application.Mappers;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.DTOs;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Application.UseCases.News.Commands.Handlers
{
    public sealed class UpdateNewsHandler : IRequestHandler<UpdateNews, NewsDto?>
    {
        private readonly ILogger<UpdateNewsHandler> _logger;
        private readonly IHttpContextService _contextService;
        private readonly IPetitionRepository _petitionRepository;
        private readonly INewsRepository _newsRepository;
        private readonly IRepository _repository;

        public UpdateNewsHandler(ILogger<UpdateNewsHandler> logger,
            IHttpContextService contextService, IPetitionRepository petitionRepository,
            INewsRepository newsRepository, IRepository repository)
        {
            _logger = logger;
            _contextService = contextService;
            _petitionRepository = petitionRepository;
            _newsRepository = newsRepository;
            _repository = repository;
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

            news.UpdateDetails(command.Title, news.PetitionId);
            await _repository.SaveChangesAsync();
            _logger.LogInformation($"News {command.Id} updated");
            return news.AsDto();
        }
    }
}
