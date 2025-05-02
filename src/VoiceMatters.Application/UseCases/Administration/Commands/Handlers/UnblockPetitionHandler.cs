using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration.Commands.Handlers
{
    public sealed class UnblockPetitionHandler : IRequestHandler<UnblockPetition>
    {
        private readonly IPetitionRepository _petitionRepository;
        private readonly ILogger<UnblockPetitionHandler> _logger;
        private readonly IRepository _repository;

        public UnblockPetitionHandler(IPetitionRepository petitionRepository,
            ILogger<UnblockPetitionHandler> logger,
            IRepository repository)
        {
            _petitionRepository = petitionRepository;
            _logger = logger;
            _repository = repository;
        }

        public async Task Handle(UnblockPetition command, CancellationToken cancellationToken)
        {
            var petition = await _petitionRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find petition {command.Id}");

            petition.IsBlocked = false;

            await _repository.SaveChangesAsync();
            _logger.LogInformation($"Petition {petition.Id} is unblocked.");
        }
    }
}
