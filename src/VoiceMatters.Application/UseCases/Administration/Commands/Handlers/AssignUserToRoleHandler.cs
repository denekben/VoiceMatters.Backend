using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Application.Services;

namespace VoiceMatters.Application.UseCases.Administration.Commands.Handlers
{
    public sealed class AssignUserToRoleHandler : IRequestHandler<AssignUserToRole>
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AssignUserToRoleHandler> _logger;

        public AssignUserToRoleHandler(IAuthService authService,
            ILogger<AssignUserToRoleHandler> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        public async Task Handle(AssignUserToRole command, CancellationToken cancellationToken)
        {
            var (id, roleName) = command;

            await _authService.AssignUserToRole(id, roleName);

            _logger.LogInformation($"User {id} assigned to role {roleName}.");
        }
    }
}
