using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Repositories;

namespace VoiceMatters.Application.UseCases.Administration.Commands.Handlers
{
    public sealed class AssignUserToRoleHandler : IRequestHandler<AssignUserToRole>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AssignUserToRoleHandler> _logger;
        private readonly IRepository _repository;

        public AssignUserToRoleHandler(IAuthService authService,
            ILogger<AssignUserToRoleHandler> logger,
            IRepository repository)
        {
            _authService = authService;
            _logger = logger;
            _repository = repository;
        }

        public async Task Handle(AssignUserToRole command, CancellationToken cancellationToken)
        {
            var (id, roleName) = command;

            await _authService.AssignUserToRole(id, roleName);

            await _repository.SaveChangesAsync();
            _logger.LogInformation($"User {id} assigned to role {roleName}.");
        }
    }
}
