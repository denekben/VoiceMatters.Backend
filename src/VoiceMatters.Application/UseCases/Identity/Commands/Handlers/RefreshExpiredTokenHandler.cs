using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using VoiceMatters.Application.Services;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Identity.Commands.Handlers
{
    public sealed class RefreshExpiredTokenHandler : IRequestHandler<RefreshExpiredToken, string?>
    {
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly ILogger<RefreshExpiredTokenHandler> _logger;
        private readonly IRoleRepository _roleRepository;

        public RefreshExpiredTokenHandler(IAuthService authService, ITokenService tokenService, ILogger<RefreshExpiredTokenHandler> logger,
            IRoleRepository roleRepository)
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
            _roleRepository = roleRepository;
        }

        public async Task<string?> Handle(RefreshExpiredToken command, CancellationToken cancellationToken)
        {
            var email = _tokenService.GetPrincipalFromExpiredToken(command.AccessToken)
                ?.Claims.SingleOrDefault(x => x.Type.Equals(ClaimTypes.Email))?.Value ??
                throw new InvalidOperationException("Cannot refresh token");

            if (!await _authService.IsRefreshTokenValid(email, command.RefreshToken))
            {
                throw new InvalidOperationException("Refresh token is invalid");
            }

            var user = await _authService.GetUserByEmailAsync(email);

            var role = await _roleRepository.GetAsync(user.Id)
                ?? throw new BadRequestException($"Cannot find role for user {user.Id}");

            string accessToken = _tokenService.GenerateAccessToken(user.Id, user.LastName, email, role.RoleName)
                ?? throw new InvalidOperationException("Cannot create access token");

            _logger.LogInformation($"User {user.Id} refreshed expired token");

            return accessToken;
        }
    }
}
