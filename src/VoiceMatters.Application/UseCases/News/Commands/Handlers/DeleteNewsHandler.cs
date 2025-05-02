using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Application.UseCases.News.Commands.Handlers
{
    public sealed class DeleteNewsHandler : IRequestHandler<DeleteNews>
    {
        private readonly ILogger<DelegatingHandler> _logger;
        private readonly INewsRepository _newsRepository;
        private readonly IHttpContextService _contextService;
        private readonly IPetitionRepository _petitionRepository;
        private readonly IRepository _repository;

        public DeleteNewsHandler(ILogger<DelegatingHandler> logger,
            INewsRepository newsRepository, IHttpContextService contextService,
            IPetitionRepository petitionRepository, IRepository repository)
        {
            _logger = logger;
            _newsRepository = newsRepository;
            _contextService = contextService;
            _petitionRepository = petitionRepository;
            _repository = repository;
        }

        public async Task Handle(DeleteNews command, CancellationToken cancellationToken)
        {
            var creatorId = _contextService.GetCurrentUserId();

            var news = await _newsRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find news {command.Id}");

            var petition = await _petitionRepository.GetAsync(news.PetitionId)
                ?? throw new BadRequestException($"Cannot find petition {news.PetitionId}");

            if (petition.CreatorId != creatorId)
                throw new BadRequestException($"Only petition creator can delete news for this petition");

            await _newsRepository.DeleteAsync(news);
            await _repository.SaveChangesAsync();
            _logger.LogInformation($"News {command.Id} deleted");
        }
    }
}
