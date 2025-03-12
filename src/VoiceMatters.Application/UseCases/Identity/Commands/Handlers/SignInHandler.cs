using VoiceMatters.Application.Services;
using VoiceMatters.Shared.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;
using VoiceMatters.Domain.Repositories;
using VoiceMatters.Shared.Exceptions;

namespace VoiceMatters.Application.UseCases.Identity.Commands.Handlers
{
    public sealed class SignInHandler : IRequestHandler<SignIn, TokensDto?>
    {
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<SignInHandler> _logger;
        private readonly IRoleRepository _roleRepository;

        public SignInHandler(IAuthService authService, ITokenService tokenService, ILogger<SignInHandler> logger,
            IRoleRepository roleRepository)
        {
            _authService = authService;
            _tokenService = tokenService;
            _logger = logger;
            _roleRepository = roleRepository;
        }

        public async Task<TokensDto?> Handle(SignIn command, CancellationToken cancellationToken)
        {
            var (email, password) = command;

            await _authService.SigninUserAsync(email, password);
            var user = await _authService.GetUserByEmailAsync(email);

            var refreshToken = _tokenService.GenerateRefreshToken();

            await _authService.UpdateRefreshToken(email, refreshToken);

            var role = await _roleRepository.GetAsync(user.Id)
                ?? throw new BadRequestException($"Cannot find role for user {user.Id}");

            string accessToken = _tokenService.GenerateAccessToken(user.Id, user.LastName, email, role.RoleName)
                ?? throw new InvalidOperationException("Cannot create access token");

            _logger.LogInformation($"User {email} signed in");
            return new TokensDto(accessToken, refreshToken);
        }
    }
}
