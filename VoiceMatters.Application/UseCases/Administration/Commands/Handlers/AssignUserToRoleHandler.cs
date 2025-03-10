using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoiceMatters.Application.Services;
using VoiceMatters.Application.UseCases.Identity.Commands.Handlers;
using VoiceMatters.Domain.Repositories;

namespace VoiceMatters.Application.UseCases.Administration.Commands.Handlers
{
    internal sealed class AssignUserToRoleHandler : IRequestHandler<AssignUserToRole>
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
