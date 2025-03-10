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
    internal sealed class UnblockUserHandler : IRequestHandler<UnblockUser>
    {
        private readonly IAppUserRepository _userRepository;
        private readonly ILogger<UnblockUserHandler> _logger;

        public UnblockUserHandler(IAppUserRepository userRepository, ILogger<UnblockUserHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task Handle(UnblockUser command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find user {command.Id}.");

            user.IsBlocked = false;

            await _userRepository.UpdateAsync(user);
            _logger.LogInformation($"User {command.Id} is unblocked.");
        }
    }
}
