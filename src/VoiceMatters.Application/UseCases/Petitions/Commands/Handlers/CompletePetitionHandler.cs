using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;
using VoiceMatters.Shared.Services;

namespace VoiceMatters.Application.UseCases.Petitions.Commands.Handlers
{
    public sealed class CompletePetitionHandler : IRequestHandler<CompletePetition>
    {
        private readonly ILogger<CompletePetitionHandler> _logger;
        private readonly IHttpContextService _contextService;
        private readonly IPetitionRepository _petitionRepository;
        private readonly IRepository _repository;

        public CompletePetitionHandler(ILogger<CompletePetitionHandler> logger,
            IHttpContextService contextService, IPetitionRepository petitionRepository,
            IRepository repository)
        {
            _logger = logger;
            _contextService = contextService;
            _petitionRepository = petitionRepository;
            _repository = repository;
        }

        public async Task Handle(CompletePetition command, CancellationToken cancellationToken)
        {
            var creatorId = _contextService.GetCurrentUserId();

            var petition = await _petitionRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find petition {command.Id}");

            if (petition.CreatorId != creatorId)
                throw new BadRequestException("Authrization error");

            if (petition.IsBlocked)
                throw new BadRequestException("Cannot complete blocked petition");

            petition.IsCompleted = true;
            petition.CompletedDate = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

            _logger.LogInformation($"Petition {petition.Id} completed");
        }
    }
}
