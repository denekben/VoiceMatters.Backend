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

        public BlockPetitionHandler(IPetitionRepository petitionRepository,
            ILogger<BlockPetitionHandler> logger)
        {
            _petitionRepository = petitionRepository;
            _logger = logger;
        }

        public async Task Handle(BlockPetition command, CancellationToken cancellationToken)
        {
            var petition = await _petitionRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find petition {command.Id}");

            petition.IsBlocked = true;

            await _petitionRepository.UpdateAsync(petition);
            _logger.LogInformation($"Petition {petition.Id} is blocked.");
        }
    }
}
