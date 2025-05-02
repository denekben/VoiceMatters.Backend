using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration.Commands.Handlers
{
    public sealed class BlockPetitionHandler : IRequestHandler<BlockPetition>
    {
        private readonly IPetitionRepository _petitionRepository;
        private readonly ILogger<BlockPetitionHandler> _logger;
        private readonly IRepository _repository;

        public BlockPetitionHandler(IPetitionRepository petitionRepository,
            ILogger<BlockPetitionHandler> logger, IRepository repository)
        {
            _petitionRepository = petitionRepository;
            _logger = logger;
            _repository = repository;
        }

        public async Task Handle(BlockPetition command, CancellationToken cancellationToken)
        {
            var petition = await _petitionRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find petition {command.Id}");

            petition.IsBlocked = true;

            await _repository.SaveChangesAsync();
            _logger.LogInformation($"Petition {petition.Id} is blocked.");
        }
    }
}
