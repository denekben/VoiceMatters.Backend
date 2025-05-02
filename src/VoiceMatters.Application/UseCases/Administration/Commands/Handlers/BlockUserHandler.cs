using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Administration.Commands.Handlers
{
    public sealed class BlockUserHandler : IRequestHandler<BlockUser>
    {
        private readonly IAppUserRepository _userRepository;
        private readonly ILogger<BlockUserHandler> _logger;
        private readonly IRepository _repository;

        public BlockUserHandler(IAppUserRepository userRepository, ILogger<BlockUserHandler> logger,
            IRepository repository)
        {
            _userRepository = userRepository;
            _logger = logger;
            _repository = repository;
        }

        public async Task Handle(BlockUser command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetAsync(command.Id)
                ?? throw new BadRequestException($"Cannot find user {command.Id}.");

            user.IsBlocked = true;

            await _repository.SaveChangesAsync();
            _logger.LogInformation($"User {command.Id} is blocked.");
        }
    }
}
