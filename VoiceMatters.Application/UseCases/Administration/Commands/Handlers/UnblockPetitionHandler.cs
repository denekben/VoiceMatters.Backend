using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration.Commands.Handlers
{
    internal sealed class UnblockPetitionHandler : IRequestHandler<UnblockPetition>
    {
        private readonly IPetitionRepository _petitionRepository;
        private readonly ILogger<UnblockPetitionHandler> _logger;

        public UnblockPetitionHandler(IPetitionRepository petitionRepository,
            ILogger<UnblockPetitionHandler> logger)
        {
            _petitionRepository = petitionRepository;
            _logger = logger;
        }

        public async Task Handle(UnblockPetition command, CancellationToken cancellationToken)
        {
            var petition = await _petitionRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find petition {command.Id}");

            petition.IsBlocked = false;

            await _petitionRepository.UpdateAsync(petition);
            _logger.LogInformation($"Petition {petition.Id} is unblocked.");
        }
    }
}
